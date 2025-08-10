import { Box, CssBaseline, CircularProgress, Typography } from '@mui/material';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import NotesPage from './pages/NotesPage';
import ProtectedRoute from './components/ProtectedRoute';
import NoteView from './components/NoteView';

function AppRoutes() {
    const { isAuthenticated, loading, login, logout } = useAuth();

    if (loading) {
        return (
            <Box 
                sx={{ 
                    display: 'flex', 
                    flexDirection: 'column',
                    alignItems: 'center', 
                    justifyContent: 'center', 
                    height: '100vh',
                    gap: 2
                }}
            >
                <CircularProgress />
                <Typography variant="body1" color="text.secondary">
                    Loading...
                </Typography>
            </Box>
        );
    }

    return (
        <Box sx={{ display: 'flex', height: '100vh' }}>
            <CssBaseline />
            <Routes>
                <Route path="/login" element={
                    <LoginPage isAuthenticated={isAuthenticated} onLogin={login} />
                } />
                <Route path="/register" element={
                    <RegisterPage isAuthenticated={isAuthenticated} onRegister={login} />
                } />
                <Route element={<ProtectedRoute isAuthenticated={isAuthenticated} />}>
                    <Route path="/" element={<NotesPage onLogout={logout} />}>
                        <Route index element={<NoteView />} />
                        <Route path=":noteId" element={<NoteView />} />
                    </Route>
                </Route>
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Box>
    );
}

export default function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <AppRoutes />
            </AuthProvider>
        </BrowserRouter>
    );
}