import React, { Component, ReactNode } from 'react';
import * as Sentry from '@sentry/react';

interface Props {
    children: ReactNode;
}

interface State {
    hasError: boolean;
    error?: Error;
}

class ErrorBoundary extends Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = { hasError: false };
    }

    static getDerivedStateFromError(error: Error): State {
        console.log('ErrorBoundary - getDerivedStateFromError:', error);
        return { hasError: true, error };
    }

    componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
        console.error('ErrorBoundary - componentDidCatch:', error, errorInfo);
        
        // Send to Sentry if available
        if (typeof Sentry !== 'undefined') {
            Sentry.withScope((scope) => {
                scope.setTag('errorBoundary', true);
                scope.setContext('errorInfo', {
                    componentStack: errorInfo.componentStack
                });
                Sentry.captureException(error);
            });
        }
    }

    handleRetry = () => {
        console.log('ErrorBoundary - Retrying...');
        this.setState({ hasError: false, error: undefined });
    };

    render() {
        if (this.state.hasError) {
            console.log('ErrorBoundary - Rendering error UI');
            return (
                <div style={{ 
                    display: 'flex', 
                    flexDirection: 'column', 
                    alignItems: 'center', 
                    justifyContent: 'center', 
                    height: '100vh',
                    padding: '40px',
                    textAlign: 'center',
                    backgroundColor: 'white',
                    fontFamily: 'system-ui, -apple-system, sans-serif'
                }}>
                    {/* Error Icon */}
                    <div style={{
                        width: '80px',
                        height: '80px',
                        borderRadius: '50%',
                        backgroundColor: '#fee',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        marginBottom: '32px'
                    }}>
                        <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="#dc3545" strokeWidth="2">
                            <circle cx="12" cy="12" r="10"/>
                            <line x1="15" y1="9" x2="9" y2="15"/>
                            <line x1="9" y1="9" x2="15" y2="15"/>
                        </svg>
                    </div>

                    <h1 style={{ 
                        color: '#333', 
                        fontSize: '28px',
                        fontWeight: '600',
                        margin: '0 0 16px 0'
                    }}>
                        Something went wrong
                    </h1>
                    
                    <p style={{ 
                        color: '#666', 
                        fontSize: '16px',
                        lineHeight: '1.5',
                        margin: '0 0 40px 0',
                        maxWidth: '500px'
                    }}>
                        We're sorry, but something unexpected happened. Please try refreshing the page or contact support if the problem persists.
                    </p>
                    
                    <div style={{ display: 'flex', gap: '16px', flexWrap: 'wrap', justifyContent: 'center' }}>
                        <button 
                            onClick={this.handleRetry}
                            style={{
                                padding: '14px 28px',
                                fontSize: '16px',
                                fontWeight: '500',
                                backgroundColor: '#007bff',
                                color: 'white',
                                border: 'none',
                                borderRadius: '8px',
                                cursor: 'pointer',
                                transition: 'background-color 0.2s',
                                minWidth: '120px'
                            }}
                            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#0056b3'}
                            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = '#007bff'}
                        >
                            Try Again
                        </button>
                        <button 
                            onClick={() => window.location.reload()}
                            style={{
                                padding: '14px 28px',
                                fontSize: '16px',
                                fontWeight: '500',
                                backgroundColor: 'white',
                                color: '#666',
                                border: '2px solid #ddd',
                                borderRadius: '8px',
                                cursor: 'pointer',
                                transition: 'all 0.2s',
                                minWidth: '120px'
                            }}
                            onMouseEnter={(e) => {
                                e.currentTarget.style.borderColor = '#bbb';
                                e.currentTarget.style.color = '#333';
                            }}
                            onMouseLeave={(e) => {
                                e.currentTarget.style.borderColor = '#ddd';
                                e.currentTarget.style.color = '#666';
                            }}
                        >
                            Refresh Page
                        </button>
                    </div>
                </div>
            );
        }

        return this.props.children;
    }
}

export default ErrorBoundary;
