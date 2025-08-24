import axios from 'axios';
import { Note } from '../types/notes';

const apiClient = axios.create({
    baseURL: process.env.REACT_APP_API_URL,
});

apiClient.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if(token){
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Auth API methods
export const authLogin = async (credentials: { email: string; password: string }) => {
    const response = await apiClient.post('auth/login', credentials);
    return response.data;
};

export const authRegister = async (userData: { email: string; password: string }) => {
    const response = await apiClient.post('auth/register', userData);
    return response.data;
};

export const authVerify = async (verificationData: { email: string; verificationCode: string }) => {
    const response = await apiClient.post('auth/verify', verificationData);
    return response.data;
};

export const authVerifyResend = async (emailData: { email: string }) => {
    const response = await apiClient.post('auth/verify-resend', emailData);
    return response.data;
};

export const authAnonymousLogin = async (payload: { GuestId?: string }) => {
    const response = await apiClient.post('auth/anonymous-login', payload);
    return response.data;
};

export const authGoogleSignin = async (googleData: { AuthorizationCode: string; RedirectUri: string }) => {
    const response = await apiClient.post('auth/google-signin', googleData);
    return response.data;
};

// Notes API methods
export const noteGetAll = async (): Promise<Note[]> => {
    const response = await apiClient.get('/note');
    return response.data;
};

export const noteCreate = async (noteData: { title: string; content: string }): Promise<Note> => {
    const response = await apiClient.post('/note', noteData);
    return response.data;
};

export const noteUpdate = async (noteData: Partial<Note>): Promise<void> => {
    await apiClient.put('/note', noteData);
};

export const noteDelete = async (noteId: string): Promise<void> => {
    await apiClient.delete(`/note/${noteId}`);
};

// Legacy export for backwards compatibility (if needed during transition)
export default apiClient;