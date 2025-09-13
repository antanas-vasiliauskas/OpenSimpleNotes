import React from 'react';
import { Box, Container, Typography, Paper, Button } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';

const PrivacyPolicyPage: React.FC = () => {
  return (
    <Box sx={{ 
      minHeight: '100vh', 
      bgcolor: 'background.default',
      py: 4
    }}>
      <Container maxWidth="md">
        <Box sx={{ mb: 3 }}>
          <Button
            component={RouterLink}
            to="/login"
            startIcon={<ArrowBackIcon />}
            sx={{ mb: 2 }}
          >
            Back to Login
          </Button>
        </Box>

        <Paper elevation={3} sx={{ p: 4 }}>
          <Typography variant="h3" component="h1" gutterBottom>
            Privacy Policy
          </Typography>
          
          <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
            <strong>Last updated:</strong> {new Date().toLocaleDateString()}
          </Typography>

          <Box sx={{ '& > *': { mb: 3 } }}>
            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Introduction
              </Typography>
              <Typography variant="body1" paragraph>
                Welcome to Open Simple Notes. We respect your privacy and are committed to protecting your personal data. 
                This privacy policy explains how we collect, use, and safeguard your information when you use our service.
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Information We Collect
              </Typography>
              <Typography variant="h6" component="h3" gutterBottom>
                Account Information
              </Typography>
              <Typography component="ul" sx={{ pl: 3 }}>
                <Typography component="li">Email address (via Google OAuth)</Typography>
                <Typography component="li">Name and profile information from your Google account</Typography>
                <Typography component="li">Profile picture (if provided by Google)</Typography>
              </Typography>
              
              <Typography variant="h6" component="h3" gutterBottom sx={{ mt: 2 }}>
                Content Data
              </Typography>
              <Typography component="ul" sx={{ pl: 3 }}>
                <Typography component="li">Notes and content you create within the application</Typography>
                <Typography component="li">Timestamps and metadata associated with your notes</Typography>
              </Typography>

              <Typography variant="h6" component="h3" gutterBottom sx={{ mt: 2 }}>
                Technical Information
              </Typography>
              <Typography component="ul" sx={{ pl: 3 }}>
                <Typography component="li">Device information and browser type</Typography>
                <Typography component="li">IP address and location data</Typography>
                <Typography component="li">Usage patterns and application interactions</Typography>
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                How We Use Your Information
              </Typography>
              <Typography component="ul" sx={{ pl: 3 }}>
                <Typography component="li">To provide and maintain our note-taking service</Typography>
                <Typography component="li">To authenticate your account via Google OAuth</Typography>
                <Typography component="li">To sync your notes across devices</Typography>
                <Typography component="li">To improve our service and user experience</Typography>
                <Typography component="li">To communicate with you about service updates</Typography>
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Data Storage and Security
              </Typography>
              <Typography variant="body1" paragraph>
                Your data is stored securely using industry-standard encryption methods. We implement appropriate 
                technical and organizational measures to protect your personal information against unauthorized 
                access, alteration, disclosure, or destruction.
              </Typography>
              <Typography variant="body1" paragraph>
                We use Google OAuth for authentication, which means we don't store your Google account password. 
                Your notes are encrypted and stored in our secure databases.
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Third-Party Services
              </Typography>
              <Typography variant="body1" paragraph>
                We use Google OAuth for authentication services. By using our service, you also agree to Google's 
                Privacy Policy. We may use analytics services to understand how our application is used, but we 
                ensure that any data shared is anonymized.
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Data Retention
              </Typography>
              <Typography variant="body1" paragraph>
                We retain your personal information only for as long as necessary to provide our services and 
                comply with legal obligations. You may delete your account at any time, which will result in 
                the permanent deletion of your notes and associated data.
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Your Rights
              </Typography>
              <Typography variant="body1" paragraph>
                You have the right to:
              </Typography>
              <Typography component="ul" sx={{ pl: 3 }}>
                <Typography component="li">Access your personal data</Typography>
                <Typography component="li">Correct inaccurate personal data</Typography>
                <Typography component="li">Request deletion of your personal data</Typography>
                <Typography component="li">Export your data</Typography>
                <Typography component="li">Withdraw consent for data processing</Typography>
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Cookies and Tracking
              </Typography>
              <Typography variant="body1" paragraph>
                We use essential cookies to maintain your session and provide core functionality. We may also 
                use analytics cookies to understand how you use our service. You can control cookie settings 
                through your browser preferences.
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Children's Privacy
              </Typography>
              <Typography variant="body1" paragraph>
                Our service is not intended for children under 13 years of age. We do not knowingly collect 
                personal information from children under 13. If you are a parent or guardian and believe your 
                child has provided us with personal information, please contact us.
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Changes to This Privacy Policy
              </Typography>
              <Typography variant="body1" paragraph>
                We may update this privacy policy from time to time. We will notify you of any changes by 
                posting the new privacy policy on this page and updating the "Last updated" date. You are 
                advised to review this privacy policy periodically for any changes.
              </Typography>
            </Box>

            <Box>
              <Typography variant="h5" component="h2" gutterBottom>
                Contact Us
              </Typography>
              <Typography variant="body1" paragraph>
                If you have any questions about this privacy policy or our privacy practices, please contact us at:
              </Typography>
              <Typography variant="body1" paragraph>
                Email: aavasiliauskas@gmail.com
              </Typography>
            </Box>
          </Box>
        </Paper>
      </Container>
    </Box>
  );
};

export default PrivacyPolicyPage;
