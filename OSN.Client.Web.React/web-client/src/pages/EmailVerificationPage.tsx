import { Navigate } from 'react-router-dom';
import EmailVerification from '../components/EmailVerification';

interface EmailVerificationPageProps {
    isAuthenticated: boolean;
    onVerify: () => void;
}

export default function EmailVerificationPage({ isAuthenticated, onVerify }: EmailVerificationPageProps) {
    if (isAuthenticated) {
        return <Navigate to="/" replace />;
    }

    return <EmailVerification onVerify={onVerify} />;
}
