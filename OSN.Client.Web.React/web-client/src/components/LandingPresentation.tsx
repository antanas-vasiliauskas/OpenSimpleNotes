import React from 'react';
import { Box, Typography } from '@mui/material';

const LandingPresentation: React.FC = () => {
    return (
        <Box 
            sx={{ 
                width: '60%',
                flexShrink: 0,
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center',
                padding: 4,
                color: 'white',
                position: 'relative'
            }}
        >
            <Box sx={{ maxWidth: 650, width: '100%', px: 2 }}>
                <Typography 
                    variant="h1" 
                    sx={{ 
                        fontSize: { xs: '2rem', md: '3rem' },
                        fontWeight: 900,
                        mb: 3,
                        lineHeight: 1.1,
                        textAlign: 'left'
                    }}
                >
                    Organize your mind -
                    <br />
                    with Open Simple Notes
                </Typography>
                
                <Typography 
                    variant="h6" 
                    sx={{ 
                        mb: 4, 
                        opacity: 0.9,
                        lineHeight: 1.6,
                        fontWeight: 400,
                        textAlign: 'left',
                        maxWidth: 600
                    }}
                >
                    Open Simple Notes is an open-source note-taking platform designed for simplicity and productivity. 
                    Capture your thoughts, organize your ideas, and access them anywhere. 
                    Built for individuals and teams who value clean, distraction-free writing experiences.
                </Typography>
                
                <Box component="a" href="#" sx={{ display: 'inline-block', mb: 6 }}>
                    <Box 
                        component="img" 
                        src="https://play.google.com/intl/en_us/badges/static/images/badges/en_badge_web_generic.png"
                        alt="Get it on Google Play"
                        sx={{ 
                            height: 80,
                            cursor: 'pointer',
                            '&:hover': {
                                transform: 'scale(1.05)',
                                transition: 'transform 0.2s'
                            }
                        }}
                    />
                </Box>
            </Box>
            
            <Typography 
                variant="caption" 
                sx={{ 
                    position: 'absolute',
                    bottom: 20,
                    left: 32,
                    opacity: 0.7,
                    fontSize: '0.7rem'
                }}
            >
                © 2025 Open Simple Notes. All rights reserved.
            </Typography>
        </Box>
    );
};

export default LandingPresentation;
