import { useState } from 'react';
import { Button } from '@mui/material';
import PersonIcon from '@mui/icons-material/Person';
import api from '../api/client';

interface GuestLoginButtonProps {
    onLogin: () => void;
    disabled?: boolean;
    onError?: (error: string) => void;
}

export default function GuestLoginButton({ onLogin, disabled = false, onError }: GuestLoginButtonProps) {
    const [loading, setLoading] = useState(false);

    const handleAnonymousLogin = async () => {
        setLoading(true);
        
        try {
            // Get existing guest ID if available
            const existingGuestId = localStorage.getItem('guestId');
            const payload: { GuestId?: string } = {};
            
            if (existingGuestId) {
                payload.GuestId = existingGuestId;
            }

            const { token, role, guestId } = await api.auth.anonymousLogin(payload);
            
            // Store the authentication data
            localStorage.setItem('token', token);
            localStorage.setItem('userRole', role);
            localStorage.setItem('guestId', guestId);
            
            onLogin();
        } catch (error: any) {
            console.error('Anonymous login failed:', error);
            const errorMessage = error.response?.data?.message || 'Anonymous login failed. Please try again.';
            if (onError) {
                onError(errorMessage);
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <Button
            onClick={handleAnonymousLogin}
            variant="outlined"
            fullWidth
            size="large"
            disabled={disabled || loading}
            sx={{
                mt: 0,
                mb: 1,
                color: 'black',
                textTransform: 'none',
                border: '1px solid rgba(0, 0, 0, 0.12)',
                backgroundColor: 'transparent',
                '&:hover': {
                    backgroundColor: 'rgba(0, 0, 0, 0.04)',
                    border: '1px solid rgba(0, 0, 0, 0.2)'
                }
            }}
        >
            <PersonIcon sx={{ width: 20, height: 20, mr: 2 }} />
            {loading ? 'Signing In...' : 'Continue as Guest'}
        </Button>
    );
}
