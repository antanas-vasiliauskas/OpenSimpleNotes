import {useState} from 'react';
import api from '../api/client';
import {Button, TextField, Box} from '@mui/material';

export default function Login({onLogin}: {onLogin: () => void}){
    const [credentials, setCredentials] = useState({email: '', password: ''});
    const handleSubmit = async (e: React.FormEvent) =>{
        e.preventDefault();
        try{
            const {data} = await api.post('auth/login', credentials);
            localStorage.setItem('token', data.token);
            onLogin();
        }catch(error) {
            alert('Login failed.');
        }
    };

    return(
        <Box component="form" onSubmit={handleSubmit} sx={{maxWidth: 400, mx: 'auto', mt: 10}}>
            <TextField
                fullWidth
                label="Email"
                value={credentials.email}
                onChange={(e) => setCredentials({...credentials, email: e.target.value})}
                margin="normal"
            />
            <TextField
                fullWidth
                label="Password"
                type="password"
                value={credentials.password}
                onChange={(e) => setCredentials({...credentials, password: e.target.value})}
                margin="normal"
            />
            <Button type="submit" variant="contained" fullWidth sx={{mt: 2}}>
                Login
            </Button>
        </Box>
    );
}
