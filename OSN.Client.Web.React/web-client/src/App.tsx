import {useState, useEffect} from 'react';
import {Box, CssBaseline} from '@mui/material';
import { BrowserRouter, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import api from './api/client';
import Login from './components/Login';
import NoteList from './components/NoteList';
import NoteView from './components/NoteView';
import {Note} from './types/notes';
import ProtectedRoute from './components/ProtectedRoute';

// TODO: Add routing instead of same page for everything

export default function App(){
    const[isAuthenticated, setIsAuthenticated] = useState(false);
    const[notes, setNotes] = useState<Note[]>([]);

    useEffect(() => {
        const token = localStorage.getItem('token');
        if(token) {
            setIsAuthenticated(true);
            loadNotes();
        }
    }, []);

    const loadNotes = async () => {
        try{
            const {data} = await api.get('/note');
            setNotes(data);
        }catch(error){
            console.error('Failed to load notes:', error);
        }
    };

    const handleLogin = () => {
        setIsAuthenticated(true);
        loadNotes();
    };



    return (
        <BrowserRouter>
            <Box sx={{display: 'flex', height: '100vh'}}>
                <CssBaseline/>
                <Routes>
                    <Route path = "/login" element={
                        isAuthenticated ? <Navigate to="/" replace /> : <Login onLogin={handleLogin} />
                    }/>
                    <Route element={<ProtectedRoute isAuthenticated={isAuthenticated}/>}>
                        <Route path="/" element={
                            <NoteLayout notes={notes}/>
                        }>
                            <Route index element={<NoteView notes={notes} />} />
                            <Route path=":noteId" element={<NoteView notes={notes} />} />
                        </Route>
                    </Route>
                    <Route path="*" element={<Navigate to="/" replace />} />
                </Routes>
            </Box>
        </BrowserRouter>
    );
}

function NoteLayout({ notes }: { notes: Note[] }) {
  return (
    <>
      <NoteList notes={notes} />
      {/* Outlet will render either the index or noteId route */}
      <Outlet />
    </>
  );
}