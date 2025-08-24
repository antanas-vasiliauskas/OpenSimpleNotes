import { useState, useEffect } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { 
    Box, 
    Typography, 
    TextField, 
    Button, 
    Alert, 
    Paper,
    InputAdornment
} from '@mui/material';
import { Email, Lock } from '@mui/icons-material';
import api from '../api/client';
import LandingPresentation from './LandingPresentation';

interface LocationState {
    email: string;
}

export default function EmailVerification({ onVerify }: { onVerify: () => void }) {
    const navigate = useNavigate();
    const location = useLocation();
    const state = location.state as LocationState;
    
    const [verificationCode, setVerificationCode] = useState('');
    const [error, setError] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [resendLoading, setResendLoading] = useState(false);
    const [resendCooldown, setResendCooldown] = useState(0);
    const [successMessage, setSuccessMessage] = useState('');

    const email = state?.email || '';

    useEffect(() => {
        if (!email) {
            navigate('/register');
        }
    }, [email, navigate]);

    useEffect(() => {
        let interval: NodeJS.Timeout;
        if (resendCooldown > 0) {
            interval = setInterval(() => {
                setResendCooldown(prev => prev - 1);
            }, 1000);
        }
        return () => clearInterval(interval);
    }, [resendCooldown]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (!verificationCode || verificationCode.length !== 6) {
            setError('Please enter a 6-digit verification code');
            return;
        }
        
        setLoading(true);
        setError('');
        
        try {
            const data = await api.auth.verify({
                email: email,
                verificationCode: verificationCode
            });
            
            localStorage.setItem('token', data.token);
            onVerify();
        } catch (error: any) {
            console.error('Verification failed:', error);
            setError(error.response?.data?.message || 'Verification failed. Please check your code and try again.');
        } finally {
            setLoading(false);
        }
    };

    const handleResend = async () => {
        setResendLoading(true);
        setError('');
        setSuccessMessage('');
        
        try {
            await api.auth.verifyResend({ email: email });
            setSuccessMessage('Verification code sent! Please check your email.');
            setResendCooldown(60); // 60 second cooldown
        } catch (error: any) {
            console.error('Resend failed:', error);
            setError(error.response?.data?.message || 'Failed to resend verification code. Please try again.');
        } finally {
            setResendLoading(false);
        }
    };

    const handleCodeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.replace(/\D/g, ''); // Only allow digits
        if (value.length <= 6) {
            setVerificationCode(value);
        }
    };

    if (!email) {
        return null;
    }

    return (
        <Box sx={{ display: 'flex', minHeight: '100vh', width: '100vw' }}>
            {/* Left side - Presentation (hidden on small screens) */}
            <LandingPresentation />

            {/* Right side - Verification Form */}
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
                            maxWidth: 400, 
                            width: '100%',
                            display: 'flex',
                            flexDirection: 'column',
                            gap: 2
                        }}
                    >
                        <Typography variant="h4" component="h1" align="center" sx={{ mb: 1, color: '#333' }}>
                            Verify Your Email
                        </Typography>
                        
                        <Paper elevation={0} sx={{ p: 3, backgroundColor: '#f5f5f5', borderRadius: 2, mb: 2 }}>
                            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                                <Email sx={{ color: '#1976d2', mr: 1 }} />
                                <Typography variant="body2" color="text.secondary">
                                    We've sent a verification code to:
                                </Typography>
                            </Box>
                            <Typography variant="body1" sx={{ fontWeight: 'bold', mb: 2 }}>
                                {email}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                Enter the 6-digit code below to verify your account. 
                                <strong> Don't forget to check your spam folder!</strong>
                            </Typography>
                        </Paper>
                        
                        {error && (
                            <Alert severity="error" sx={{ mb: 2 }}>
                                {error}
                            </Alert>
                        )}
                        
                        {successMessage && (
                            <Alert severity="success" sx={{ mb: 2 }}>
                                {successMessage}
                            </Alert>
                        )}
                        
                        <TextField
                            fullWidth
                            label="Verification Code"
                            value={verificationCode}
                            onChange={handleCodeChange}
                            disabled={loading}
                            required
                            inputProps={{
                                maxLength: 6,
                                style: { 
                                    textAlign: 'center', 
                                    fontSize: '1.2rem', 
                                    letterSpacing: '0.5rem',
                                    fontFamily: 'monospace'
                                }
                            }}
                            InputProps={{
                                startAdornment: (
                                    <InputAdornment position="start">
                                        <Lock />
                                    </InputAdornment>
                                ),
                            }}
                            placeholder="000000"
                            helperText="Enter the 6-digit code sent to your email"
                        />
                        
                        <Button 
                            type="submit" 
                            variant="contained" 
                            fullWidth 
                            size="large"
                            disabled={loading || verificationCode.length !== 6}
                            sx={{ mt: 1 }}
                        >
                            {loading ? 'Verifying...' : 'Verify Email'}
                        </Button>
                        
                        <Box sx={{ textAlign: 'center', mt: 2 }}>
                            <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                                Didn't receive the code?
                            </Typography>
                            <Button
                                variant="text"
                                onClick={handleResend}
                                disabled={resendLoading || resendCooldown > 0}
                                size="small"
                            >
                                {resendLoading ? 'Sending...' : 
                                 resendCooldown > 0 ? `Resend in ${resendCooldown}s` : 
                                 'Resend Code'}
                            </Button>
                        </Box>
                        
                        <Typography variant="body2" align="center" sx={{ mt: 2 }}>
                            Need to change your email?{' '}
                            <Link 
                                to="/register" 
                                style={{ 
                                    color: '#1976d2', 
                                    textDecoration: 'none',
                                    fontWeight: 500
                                }}
                            >
                                Go back to registration
                            </Link>
                        </Typography>
                    </Box>
                </Box>
            </Box>
        </Box>
    );
}
