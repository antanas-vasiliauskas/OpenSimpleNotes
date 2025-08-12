import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Button, TextField, Box, Typography, Divider, Alert } from '@mui/material';
import api from '../api/client';
import LandingPresentation from './LandingPresentation';
import GuestLoginButton from './GuestLoginButton';
import GoogleSignInButton from './GoogleSignInButton';

export default function Login({ onLogin }: { onLogin: () => void }) {
    const [credentials, setCredentials] = useState({ email: '', password: '' });
    const [error, setError] = useState<string>('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        
        try {
            const response = await api.post('auth/login', credentials);
            const {token, role} = response.data;
            localStorage.setItem('token', token);
            localStorage.setItem('userRole', role);
            onLogin();
        } catch (error: any) {
            console.error('Login failed:', error);
            setError(error.response?.data?.message || 'Login failed. Please check your credentials.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box sx={{ display: 'flex', minHeight: '100vh', width: '100vw' }}>
            {/* Left side - Presentation (hidden on small screens) */}
            <LandingPresentation />

            {/* Right side - Login Form */}
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
                            Sign In
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
                            value={credentials.email}
                            onChange={(e) => setCredentials({ ...credentials, email: e.target.value })}
                            disabled={loading}
                            required
                        />
                        
                        <TextField
                            fullWidth
                            label="Password"
                            type="password"
                            value={credentials.password}
                            onChange={(e) => setCredentials({ ...credentials, password: e.target.value })}
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
                            {loading ? 'Signing In...' : 'Sign In'}
                        </Button>
                        
                        <Divider sx={{ my: 2 }}>or</Divider>
                        
                        <GoogleSignInButton 
                            onLogin={onLogin}
                            disabled={loading}
                            onError={setError}
                        />
                        
                        <GuestLoginButton 
                            onLogin={onLogin}
                            disabled={loading}
                            onError={setError}
                        />
                        
                        <Typography variant="body2" align="center" sx={{ mt: 2 }}>
                            Don't have an account yet?{' '}
                            <Link 
                                to="/register" 
                                style={{ 
                                    color: '#1976d2', 
                                    textDecoration: 'none',
                                    fontWeight: 500
                                }}
                            >
                                Register now
                            </Link>
                        </Typography>
                    </Box>
                </Box>
            </Box>
        </Box>
    );
}
