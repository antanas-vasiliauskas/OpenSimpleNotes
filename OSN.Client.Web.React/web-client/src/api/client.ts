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

// API object with organized methods
const api = {
    auth: {
        login: async (credentials: { email: string; password: string }) => {
            const response = await apiClient.post('auth/login', credentials);
            return response.data;
        },

        register: async (userData: { email: string; password: string }) => {
            const response = await apiClient.post('auth/register', userData);
            return response.data;
        },

        verify: async (verificationData: { email: string; verificationCode: string }) => {
            const response = await apiClient.post('auth/verify', verificationData);
            return response.data;
        },

        verifyResend: async (emailData: { email: string }) => {
            const response = await apiClient.post('auth/verify-resend', emailData);
            return response.data;
        },

        anonymousLogin: async (payload: { GuestId?: string }) => {
            const response = await apiClient.post('auth/anonymous-login', payload);
            return response.data;
        },

        googleSignin: async (googleData: { AuthorizationCode: string; RedirectUri: string }) => {
            const response = await apiClient.post('auth/google-signin', googleData);
            return response.data;
        },
    },

    note: {
        getAll: async (): Promise<Note[]> => {
            const response = await apiClient.get('/note');
            return response.data;
        },

        getById: async (noteId: string): Promise<Note> => {
            const response = await apiClient.get(`/note/${noteId}`);
            return response.data;
        },

        create: async (noteData: { title: string; content: string }): Promise<Note> => {
            const response = await apiClient.post('/note', noteData);
            return response.data;
        },

        update: async (noteData: Partial<Note>): Promise<Partial<Note>> => {
            const response = await apiClient.put('/note', noteData);
            return response.data;
        },

        delete: async (noteId: string): Promise<void> => {
            await apiClient.delete(`/note/${noteId}`);
        },
    },
};

export default api;