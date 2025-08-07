import { useState, useEffect, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { Button, TextField, Box, Typography, Divider, Alert } from '@mui/material';
import api from '../api/client';
import LandingPresentation from './LandingPresentation';
import { initGoogleAuth, promptGoogleSignIn } from '../utils/googleAuth';

export default function Register({ onRegister }: { onRegister: () => void }) {
    const [formData, setFormData] = useState({ 
        email: '', 
        password: '', 
        confirmPassword: '' 
    });
    const [error, setError] = useState<string>('');
    const [loading, setLoading] = useState(false);

    const handleGoogleSuccess = useCallback(async (idToken: string) => {
        setLoading(true);
        setError('');
        
        try {
            const response = await api.post('auth/google-signin', {
                IdToken: idToken
            });
            
            const { token, role } = response.data;
            localStorage.setItem('token', token);
            localStorage.setItem('userRole', role);
            onRegister();
        } catch (error: any) {
            console.error('Google sign-up failed:', error);
            setError(error.response?.data?.message || 'Google sign-up failed. Please try again.');
        } finally {
            setLoading(false);
        }
    }, [onRegister]);

    useEffect(() => {
        // Initialize Google Auth when component mounts
        const initGoogle = () => {
            initGoogleAuth(handleGoogleSuccess);
        };

        if (window.google) {
            initGoogle();
        } else {
            const script = document.querySelector('script[src="https://accounts.google.com/gsi/client"]');
            if (script) {
                script.addEventListener('load', initGoogle);
            }
        }
    }, [handleGoogleSuccess]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }
        
        setLoading(true);
        setError('');
        
        try {
            const { data } = await api.post('auth/register', {
                email: formData.email,
                password: formData.password
            });
            localStorage.setItem('token', data.token);
            onRegister();
        } catch (error: any) {
            console.error('Registration failed:', error);
            setError(error.response?.data?.message || 'Registration failed. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    const handleGoogleSignUp = () => {
        if (window.google) {
            promptGoogleSignIn();
        } else {
            setError('Google Sign-up is not available. Please try again later.');
        }
    };

    return (
        <Box sx={{ display: 'flex', minHeight: '100vh', width: '100vw' }}>
            {/* Left side - Presentation (60%) */}
            <LandingPresentation />

            {/* Right side - Register Form (40%) */}
            <Box 
                sx={{ 
                    width: '40%',
                    flexShrink: 0,
                    backgroundColor: 'white',
                    display: 'flex',
                    flexDirection: 'column',
                    position: 'relative'
                }}
            >
                {/* Logo at top left */}
                <Box sx={{ position: 'absolute', top: 16, left: 16 }}>
                    <Typography variant="h6" sx={{ fontWeight: 'bold', color: '#333' }}>
                        Open Simple Notes
                    </Typography>
                </Box>
                
                {/* Centered form */}
                <Box 
                    sx={{ 
                        display: 'flex', 
                        flexDirection: 'column', 
                        justifyContent: 'center', 
                        alignItems: 'center', 
                        flex: 1,
                        padding: 4
                    }}
                >
                    <Box 
                        component="form" 
                        onSubmit={handleSubmit} 
                        sx={{ 
                            maxWidth: 350, 
                            width: '100%',
                            display: 'flex',
                            flexDirection: 'column',
                            gap: 2
                        }}
                    >
                        <Typography variant="h4" component="h1" align="center" sx={{ mb: 3, color: '#333' }}>
                            Create Account
                        </Typography>
                        
                        {error && (
                            <Alert severity="error" sx={{ mb: 2 }}>
                                {error}
                            </Alert>
                        )}
                        
                        <TextField
                            fullWidth
                            label="Email"
                            type="email"
                            value={formData.email}
                            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                            disabled={loading}
                            required
                        />
                        
                        <TextField
                            fullWidth
                            label="Password"
                            type="password"
                            value={formData.password}
                            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                            disabled={loading}
                            required
                        />
                        
                        <TextField
                            fullWidth
                            label="Confirm Password"
                            type="password"
                            value={formData.confirmPassword}
                            onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                            disabled={loading}
                            required
                        />
                        
                        <Button 
                            type="submit" 
                            variant="contained" 
                            fullWidth 
                            size="large"
                            disabled={loading}
                            sx={{ mt: 1 }}
                        >
                            {loading ? 'Creating Account...' : 'Create Account'}
                        </Button>
                        
                        <Divider sx={{ my: 2 }}>or</Divider>
                        
                        <Button
                            onClick={handleGoogleSignUp}
                            variant="outlined"
                            fullWidth
                            size="large"
                            disabled={loading}
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
                            Continue with Google
                        </Button>
                        
                        <Typography variant="body2" align="center" sx={{ mt: 3 }}>
                            Already have an account?{' '}
                            <Link 
                                to="/login" 
                                style={{ 
                                    color: '#1976d2', 
                                    textDecoration: 'none',
                                    fontWeight: 500
                                }}
                            >
                                Sign in
                            </Link>
                        </Typography>
                    </Box>
                </Box>
            </Box>
        </Box>
    );
}
