import axios from 'axios';

const api = axios.create({
    baseURL: process.env.REACT_APP_API_URL,
    withCredentials: true,
});

// Response interceptor to handle authentication errors
api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // If we get a 401, the user is no longer authenticated
            window.dispatchEvent(new CustomEvent('auth-error'));
        }
        return Promise.reject(error);
    }
);

export default api;