import React, { createContext, useContext, useState, ReactNode, useEffect } from 'react';
import { Snackbar, Alert } from '@mui/material';
import { Error as ErrorIcon } from '@mui/icons-material';

interface ToastContextType {
    showError: (message: string) => void;
}

const ToastContext = createContext<ToastContextType | undefined>(undefined);

// Global reference for Sentry to use
let globalShowError: ((message: string) => void) | null = null;

export const showGlobalError = (message: string) => {
    if (globalShowError) {
        globalShowError(message);
    }
};

export const useToast = () => {
    const context = useContext(ToastContext);
    if (!context) {
        throw new Error('useToast must be used within a ToastProvider');
    }
    return context;
};

interface ToastProviderProps {
    children: ReactNode;
}

export const ToastProvider: React.FC<ToastProviderProps> = ({ children }) => {
    const [error, setError] = useState<string | null>(null);

    const showError = (message: string) => {
        setError(message);
    };

    useEffect(() => {
        // Set global reference when component mounts
        globalShowError = showError;
        
        // Cleanup on unmount
        return () => {
            globalShowError = null;
        };
    }, []);

    const handleClose = () => {
        setError(null);
    };

    return (
        <ToastContext.Provider value={{ showError }}>
            {children}
            <Snackbar
                open={!!error}
                autoHideDuration={6000}
                onClose={handleClose}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
                sx={{ width: '600px', maxWidth: '90vw' }}
            >
                <Alert
                    onClose={handleClose}
                    severity="error"
                    variant="filled"
                    icon={<ErrorIcon />}
                    sx={{ 
                        width: '100%',
                        minWidth: '400px',
                        '& .MuiAlert-icon': {
                            fontSize: '24px'
                        }
                    }}
                >
                    {error}
                </Alert>
            </Snackbar>
        </ToastContext.Provider>
    );
};
