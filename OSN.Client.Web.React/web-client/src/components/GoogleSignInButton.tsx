import { useState, useEffect, useCallback } from 'react';
import { Button, Box } from '@mui/material';
import api from '../api/client';
import { initGoogleAuth, promptGoogleSignIn } from '../utils/googleAuth';

interface GoogleSignInButtonProps {
    onLogin: () => void;
    disabled?: boolean;
    onError?: (error: string) => void;
    buttonText?: string;
}

export default function GoogleSignInButton({ 
    onLogin, 
    disabled = false, 
    onError,
    buttonText = 'Continue with Google'
}: GoogleSignInButtonProps) {
    const [loading, setLoading] = useState(false);

    const handleGoogleSuccess = useCallback(async (authorizationCode: string) => {
        setLoading(true);
        
        try {
            const response = await api.post('auth/google-signin', {
                AuthorizationCode: authorizationCode,
                RedirectUri: `${window.location.origin}/oauth-callback.html`
            });
            
            const { token, role } = response.data;
            localStorage.setItem('token', token);
            localStorage.setItem('userRole', role);
            onLogin();
        } catch (error: any) {
            console.error('Google sign-in failed:', error);
            const errorMessage = error.response?.data?.message || 'Google sign-in failed. Please try again.';
            if (onError) {
                onError(errorMessage);
            }
        } finally {
            setLoading(false);
        }
    }, [onLogin, onError]);

    useEffect(() => {
        initGoogleAuth(handleGoogleSuccess);
    }, [handleGoogleSuccess]);

    const handleGoogleSignIn = () => {
        promptGoogleSignIn();
    };

    return (
        <Button
            onClick={handleGoogleSignIn}
            variant="outlined"
            fullWidth
            size="large"
            disabled={disabled || loading}
            sx={{
                backgroundColor: 'white',
                color: 'black',
                border: '1px solid #dadce0',
                textTransform: 'none',
                '&:hover': {
                    backgroundColor: '#f8f9fa',
                    border: '1px solid #dadce0'
                },
                '&:disabled': {
                    backgroundColor: '#f8f9fa',
                    border: '1px solid #dadce0'
                }
            }}
        >
            <Box component="img" 
                src="https://developers.google.com/identity/images/g-logo.png" 
                alt="Google" 
                sx={{ width: 20, height: 20, mr: 2 }} 
            />
            {loading ? 'Signing In...' : buttonText}
        </Button>
    );
}
