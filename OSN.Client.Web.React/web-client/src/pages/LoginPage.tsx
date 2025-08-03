import Login from '../components/Login';
import { Navigate } from 'react-router-dom';

interface LoginPageProps {
    isAuthenticated: boolean;
    onLogin: () => void;
}

export default function LoginPage({ isAuthenticated, onLogin }: LoginPageProps) {
    if (isAuthenticated) {
        return <Navigate to="/" replace />;
    }

    return <Login onLogin={onLogin} />;
}
