import { useState, useEffect } from 'react';
import { Box, styled } from '@mui/material';
import { Note } from '../types/notes';
import { Outlet, useNavigate } from 'react-router-dom';
import api from '../api/client';
import { isTokenExpired } from '../utils/auth';
import Sidebar from '../components/Sidebar';

const PageContainer = styled(Box)({
    display: 'flex',
    height: '100vh',
    width: '100%',
});

const MainContent = styled(Box)({
    flex: 1,
    overflow: 'auto',
    display: 'flex',
});

interface NotesPageProps {
    onLogout: () => void;
}

export default function NotesPage({ onLogout }: NotesPageProps) {
    const navigate = useNavigate();
    const [notes, setNotes] = useState<Note[]>([]);

    useEffect(() => {
        const validateAndLoadNotes = async () => {
            const token = localStorage.getItem('token');
            if (token) {
                if (isTokenExpired(token)) {
                    onLogout();
                    return;
                }

                try {
                    await loadNotes();
                } catch (error: any) {
                    if (error.response?.status === 401) {
                        onLogout();
                    } else {
                        console.error('Failed to load notes:', error);
                    }
                }
            }
        };

        validateAndLoadNotes();
    }, [onLogout]);

    const loadNotes = async () => {
        try {
            const { data } = await api.get('/note');
            setNotes(data);
        } catch (error) {
            console.error('Failed to load notes:', error);
            throw error;
        }
    };

    const handleCreateNote = async () => {
        try {
            const { data } = await api.post('/note', {
                title: 'New note',
                content: ''
            });
            setNotes(prev => [data, ...prev]);
            navigate(`/${data.id}`);
        } catch (error) {
            console.error('Failed to create note:', error);
        }
    };

    const handleDeleteNote = async (noteId: string) => {
        try {
            await api.delete(`/note/${noteId}`);
            setNotes(prev => prev.filter(note => note.id !== noteId));
        } catch (error) {
            console.error('Failed to delete note:', error);
        }
    };

    const handleUpdateNote = (noteId: string, updates: Partial<Note>) => {
        setNotes(prev => prev.map(note => 
            note.id === noteId ? { ...note, ...updates } : note
        ));
    };

    return (
        <PageContainer>
            <Sidebar
                notes={notes}
                onCreateNote={handleCreateNote}
                onDeleteNote={handleDeleteNote}
                onLogout={onLogout}
            />
            <MainContent>
                <Outlet context={{ notes, onUpdateNote: handleUpdateNote }} />
            </MainContent>
        </PageContainer>
    );
}
