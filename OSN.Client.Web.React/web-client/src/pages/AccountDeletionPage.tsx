import React, { useState } from 'react';
import { 
  Box, 
  Container, 
  Typography, 
  Paper, 
  Button, 
  TextField, 
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Checkbox,
  FormControlLabel
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import WarningIcon from '@mui/icons-material/Warning';

const AccountDeletionPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [reason, setReason] = useState('');
  const [confirmationChecked, setConfirmationChecked] = useState(false);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!email || !confirmationChecked) {
      setError('Please fill in all required fields and confirm deletion.');
      return;
    }

    setShowConfirmDialog(true);
  };

  const handleConfirmDeletion = async () => {
    setLoading(true);
    setError('');
    
    try {
      // Here you would typically make an API call to request account deletion
      // For now, we'll simulate a successful submission
      await new Promise(resolve => setTimeout(resolve, 1000)); // Simulate API call
      
      setSubmitted(true);
      setShowConfirmDialog(false);
    } catch (error: any) {
      setError('Failed to submit deletion request. Please try again later.');
      setShowConfirmDialog(false);
    } finally {
      setLoading(false);
    }
  };

  if (submitted) {
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

          <Paper elevation={3} sx={{ p: 4, textAlign: 'center' }}>
            <Box sx={{ color: 'success.main', mb: 3 }}>
              <Typography variant="h4" component="h1" gutterBottom>
                Request Submitted
              </Typography>
            </Box>
            
            <Typography variant="body1" paragraph>
              Your account deletion request has been submitted successfully.
            </Typography>
            
            <Typography variant="body1" paragraph>
              We will process your request within 30 days as required by data protection regulations. 
              You will receive a confirmation email at <strong>{email}</strong> once the deletion is complete.
            </Typography>
            
            <Typography variant="body2" color="text.secondary" sx={{ mt: 3 }}>
              If you change your mind, you can still log in to your account until the deletion is processed.
            </Typography>
          </Paper>
        </Container>
      </Box>
    );
  }

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
          <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
            <WarningIcon sx={{ color: 'warning.main', mr: 2, fontSize: 32 }} />
            <Typography variant="h4" component="h1">
              Request Account Deletion
            </Typography>
          </Box>
          
          <Alert severity="warning" sx={{ mb: 4 }}>
            <Typography variant="body2">
              <strong>Warning:</strong> This action cannot be undone. All your notes, account data, and personal 
              information will be permanently deleted from our servers within 30 days.
            </Typography>
          </Alert>

          <Box sx={{ '& > *': { mb: 3 } }}>
            <Box>
              <Typography variant="h6" component="h2" gutterBottom>
                What will be deleted:
              </Typography>
              <Typography component="ul" sx={{ pl: 3 }}>
                <Typography component="li">Your account and profile information</Typography>
                <Typography component="li">All notes and content you've created</Typography>
                <Typography component="li">Account settings and preferences</Typography>
                <Typography component="li">Any associated metadata and timestamps</Typography>
              </Typography>
            </Box>

            <Box>
              <Typography variant="h6" component="h2" gutterBottom>
                Before you proceed:
              </Typography>
              <Typography component="ul" sx={{ pl: 3 }}>
                <Typography component="li">Download any notes you want to keep</Typography>
                <Typography component="li">Consider if you might want to use the service again in the future</Typography>
              </Typography>
            </Box>

            <Box component="form" onSubmit={handleSubmit} sx={{ mt: 4 }}>
              <Typography variant="h6" component="h2" gutterBottom>
                Deletion Request Form
              </Typography>
              
              {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                  {error}
                </Alert>
              )}
              
              <TextField
                fullWidth
                label="Email Address"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                sx={{ mb: 2 }}
                helperText="Enter the email address associated with your account"
              />
              
              <TextField
                fullWidth
                label="Reason for deletion (optional)"
                multiline
                rows={4}
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                sx={{ mb: 2 }}
                helperText="Help us improve by sharing why you're leaving (optional)"
              />
              
              <FormControlLabel
                control={
                  <Checkbox
                    checked={confirmationChecked}
                    onChange={(e) => setConfirmationChecked(e.target.checked)}
                    color="error"
                  />
                }
                label={
                  <Typography variant="body2">
                    I understand that this action is permanent and irreversible. 
                    All my data will be permanently deleted within 30 days.
                  </Typography>
                }
                sx={{ mb: 3 }}
              />
              
              <Button
                type="submit"
                variant="contained"
                color="error"
                size="large"
                startIcon={<DeleteForeverIcon />}
                disabled={!email || !confirmationChecked}
                sx={{ mr: 2 }}
              >
                Request Account Deletion
              </Button>
              
              <Button
                component={RouterLink}
                to="/login"
                variant="outlined"
                size="large"
              >
                Cancel
              </Button>
            </Box>
          </Box>
        </Paper>
      </Container>

      {/* Confirmation Dialog */}
      <Dialog
        open={showConfirmDialog}
        onClose={() => !loading && setShowConfirmDialog(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle sx={{ display: 'flex', alignItems: 'center' }}>
          <WarningIcon sx={{ color: 'error.main', mr: 1 }} />
          Final Confirmation
        </DialogTitle>
        <DialogContent>
          <Typography variant="body1" paragraph>
            Are you absolutely sure you want to delete your account and all associated data?
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Email: <strong>{email}</strong>
          </Typography>
          <Typography variant="body2" color="error.main" sx={{ mt: 2 }}>
            This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button 
            onClick={() => setShowConfirmDialog(false)}
            disabled={loading}
          >
            Cancel
          </Button>
          <Button
            onClick={handleConfirmDeletion}
            color="error"
            variant="contained"
            disabled={loading}
            startIcon={<DeleteForeverIcon />}
          >
            {loading ? 'Processing...' : 'Delete My Account'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AccountDeletionPage;
