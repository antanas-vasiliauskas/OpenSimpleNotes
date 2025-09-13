import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button, TextField, Box, Typography, Divider, Alert } from '@mui/material';
import api from '../api/client';
import LandingPresentation from './LandingPresentation';
import GuestLoginButton from './GuestLoginButton';
import GoogleSignInButton from './GoogleSignInButton';

export default function Register({ onRegister }: { onRegister: () => void }) {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({ 
        email: '', 
        password: '', 
        confirmPassword: '' 
    });
    const [error, setError] = useState<string>('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }
        
        setLoading(true);
        setError('');
        
        try {
            const { message } = await api.auth.register({
                email: formData.email,
                password: formData.password
            });
            console.log(message);
            
            // Navigate to email verification with the user's email
            navigate('/verify-email', { 
                state: { email: formData.email } 
            });
        } catch (error: any) {
            console.error('Registration failed:', error);
            setError(error.response?.data?.message || 'Registration failed. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box sx={{ display: 'flex', minHeight: '100vh', width: '100vw' }}>
            {/* Left side - Presentation (hidden on small screens) */}
            <LandingPresentation />

            {/* Right side - Register Form */}
            <Box 
                sx={{ 
                    width: { xs: '100%', md: '40%' },
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
                        
                        <GoogleSignInButton 
                            onLogin={onRegister}
                            disabled={loading}
                            onError={setError}
                            buttonText="Continue with Google"
                        />
                        
                        <GuestLoginButton 
                            onLogin={onRegister}
                            disabled={loading}
                            onError={setError}
                        />
                        
                        <Typography variant="body2" align="center" sx={{ mt: 2 }}>
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
                        
                        <Typography variant="caption" align="center" sx={{ mt: 2, color: 'text.secondary' }}>
                            By creating an account, you agree to our{' '}
                            <Link 
                                to="/privacy-policy" 
                                style={{ 
                                    color: '#1976d2', 
                                    textDecoration: 'none'
                                }}
                            >
                                Privacy Policy
                            </Link>
                        </Typography>
                    </Box>
                </Box>
            </Box>
        </Box>
    );
}
