import Register from '../components/Register';
import { Navigate } from 'react-router-dom';

interface RegisterPageProps {
    isAuthenticated: boolean;
    onRegister: () => void;
}

export default function RegisterPage({ isAuthenticated, onRegister }: RegisterPageProps) {
    if (isAuthenticated) {
        return <Navigate to="/" replace />;
    }

    return <Register onRegister={onRegister} />;
}
