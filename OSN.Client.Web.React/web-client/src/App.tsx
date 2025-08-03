import { Box, CssBaseline } from '@mui/material';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import NotesPage from './pages/NotesPage';
import ProtectedRoute from './components/ProtectedRoute';
import NoteView from './components/NoteView';

function AppRoutes() {
    const { isAuthenticated, login, logout } = useAuth();

    return (
        <Box sx={{ display: 'flex', height: '100vh' }}>
            <CssBaseline />
            <Routes>
                <Route path="/login" element={
                    <LoginPage isAuthenticated={isAuthenticated} onLogin={login} />
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