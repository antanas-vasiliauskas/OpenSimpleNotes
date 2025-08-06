import { useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/client';
import { Button, TextField, Box, Typography, Divider } from '@mui/material';
import LandingPresentation from './LandingPresentation';

export default function Register({ onRegister }: { onRegister: () => void }) {
    const [formData, setFormData] = useState({ 
        email: '', 
        password: '', 
        confirmPassword: '' 
    });
    
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (formData.password !== formData.confirmPassword) {
            alert('Passwords do not match');
            return;
        }
        
        try {
            const { data } = await api.post('auth/register', {
                email: formData.email,
                password: formData.password
            });
            localStorage.setItem('token', data.token);
            onRegister();
        } catch (error) {
            alert('Registration failed.');
        }
    };

    const handleGoogleSignUp = () => {
        // TODO: Implement Google sign-up functionality
        console.log('Google sign-up clicked');
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
                        
                        <TextField
                            fullWidth
                            label="Email"
                            type="email"
                            value={formData.email}
                            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                            required
                        />
                        
                        <TextField
                            fullWidth
                            label="Password"
                            type="password"
                            value={formData.password}
                            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                            required
                        />
                        
                        <TextField
                            fullWidth
                            label="Confirm Password"
                            type="password"
                            value={formData.confirmPassword}
                            onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                            required
                        />
                        
                        <Button 
                            type="submit" 
                            variant="contained" 
                            fullWidth 
                            size="large"
                            sx={{ mt: 1 }}
                        >
                            Create Account
                        </Button>
                        
                        <Divider sx={{ my: 2 }}>or</Divider>
                        
                        <Button
                            onClick={handleGoogleSignUp}
                            variant="outlined"
                            fullWidth
                            size="large"
                            sx={{
                                backgroundColor: 'white',
                                color: 'black',
                                border: '1px solid #dadce0',
                                textTransform: 'none',
                                '&:hover': {
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
                            Sign up with Google
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
