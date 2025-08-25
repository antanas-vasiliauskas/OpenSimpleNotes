import * as Sentry from '@sentry/react';
import { showGlobalError } from '../context/ToastContext';

export const initSentry = () => {
    const sentryDsn = process.env.REACT_APP_SENTRY_DSN;
    const sentryEnv = process.env.REACT_APP_SENTRY_ENVIRONMENT;
    const isDevelopment = sentryEnv === 'development';
    console.log(sentryDsn);

    Sentry.init({
        dsn: sentryDsn,
        environment: sentryEnv,
        release: "1.0.0",
        tracesSampleRate: 1.0,
        profilesSampleRate: 1.0,
        sendDefaultPii: isDevelopment,
        attachStacktrace: true,
        maxBreadcrumbs: 100,
        debug: isDevelopment,
        beforeSend(event) {
            // Only log error level and above
            if (event.level && !['error', 'fatal'].includes(event.level)) {
                return null;
            }
            
            // Filter out errors that shouldn't be logged
            const error = event.exception?.values?.[0];
            if (error?.value) {
                /*
                const message = error.value.toLowerCase();
                
                // Skip common client-side errors that aren't actionable

                const ignoredErrors = [
                    'network error',
                    'fetch error',
                    'loading chunk',
                    'script error',
                    'cancelled',
                    'aborted',
                    'failed to fetch'
                ];
                
                if (ignoredErrors.some(ignored => message.includes(ignored))) {
                    return null;
                }
                */
            }
            
            // Skip errors from browser extensions
            if (event.exception?.values?.[0]?.stacktrace?.frames?.some(
                frame => frame.filename?.includes('extension://')
            )) {
                return null;
            }
            const errorMessage = error?.value || 'An unexpected error occurred';
            showGlobalError(errorMessage);
            
            return event;
        },
        beforeBreadcrumb(breadcrumb) {
            // Set breadcrumb level to debug
            breadcrumb.level = 'debug';
            return breadcrumb;
        }
    });
    
};

export default Sentry;
