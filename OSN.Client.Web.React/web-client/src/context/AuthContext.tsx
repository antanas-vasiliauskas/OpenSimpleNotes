import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import api from '../api/client';
import { clearUserData, getUserRole } from '../utils/user';

interface User {
    role: string | null;
    // Add other user properties as needed
}

interface AuthContextType {
    isAuthenticated: boolean;
    loading: boolean;
    user: User;
    login: () => void;
    logout: () => Promise<void>;
    checkAuth: () => Promise<boolean>;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [loading, setLoading] = useState(true);
    const [user, setUser] = useState<User>({ role: null });

    const checkAuth = async (): Promise<boolean> => {
        try {
            // Try to make an authenticated request to verify if we're logged in
            // You can use any endpoint that requires authentication
            // If your backend has an /auth/me endpoint, use that
            // Otherwise, use an existing endpoint like /note (which likely requires auth)
            const response = await api.get('/note');
            
            // Update user info from localStorage if authentication is successful
            if (response.status === 200) {
                setUser({ role: getUserRole() });
                return true;
            }
            return false;
        } catch (error: any) {
            // If we get a 401 or any error, consider the user not authenticated
            return false;
        }
    };

    const login = () => {
        setIsAuthenticated(true);
        setUser({ role: getUserRole() });
    };

    const logout = async () => {
        try {
            // Call logout endpoint to clear the cookie
            await api.post('auth/logout');
        } catch (error) {
            console.error('Logout request failed:', error);
        } finally {
            // Clean up user data and update state
            clearUserData();
            setUser({ role: null });
            setIsAuthenticated(false);
        }
    };

    useEffect(() => {
        const initializeAuth = async () => {
            setLoading(true);
            const authenticated = await checkAuth();
            setIsAuthenticated(authenticated);
            setLoading(false);
        };

        // Listen for authentication errors from API interceptor
        const handleAuthError = () => {
            setIsAuthenticated(false);
            clearUserData();
            setUser({ role: null });
        };

        window.addEventListener('auth-error', handleAuthError);
        initializeAuth();

        return () => {
            window.removeEventListener('auth-error', handleAuthError);
        };
    }, []);

    return (
        <AuthContext.Provider value={{ isAuthenticated, loading, user, login, logout, checkAuth }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}
