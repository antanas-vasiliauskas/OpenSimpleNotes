import React from 'react';
import { Box, Typography } from '@mui/material';

const LandingPresentation: React.FC = () => {
    return (
        <Box 
            sx={{ 
                // Hide completely on small screens; show starting at md breakpoint
                display: { xs: 'none', md: 'flex' },
                width: { md: '60%' },
                flexShrink: 0,
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
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
                
                <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', alignItems: 'center', mb: 6 }}>
                    <Box component="a" href="#" aria-label="Get it on Google Play" sx={{ display: 'inline-block' }}>
                        <Box 
                            component="img" 
                            src="/img/google_play_img.png"
                            alt="Get it on Google Play"
                            sx={{ 
                                height: 62,
                                width: 'auto',
                                cursor: 'pointer',
                                '&:hover': {
                                    transform: 'scale(1.05)',
                                    transition: 'transform 0.2s'
                                }
                            }}
                        />
                    </Box>
                    {/* GitHub button matching other buttons */}
                    <Box 
                        component="a" 
                        href="https://github.com/antanas-vasiliauskas/OpenSimpleNotes" 
                        target="_blank" 
                        rel="noopener noreferrer"
                        title="Source code on GitHub"
                        sx={{ display: 'inline-block' }}
                    >
                        <Box
                            component="img"
                            src="/img/github_img.png"
                            alt="GitHub Logo"
                            sx={{ 
                                height: 62,
                                width: 'auto',
                                cursor: 'pointer',
                                '&:hover': {
                                    transform: 'scale(1.05)',
                                    transition: 'transform 0.2s'
                                }
                            }}
                        />
                    </Box>
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
