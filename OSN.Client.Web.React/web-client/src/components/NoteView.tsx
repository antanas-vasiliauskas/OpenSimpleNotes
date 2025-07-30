import {Typography, Paper, Box} from '@mui/material';
import {Note} from "../types/notes";
import { useParams } from 'react-router-dom';

export default function NoteView({ notes }: { notes: Note[] }) {
    const { noteId } = useParams();
    const selectedNote = notes.find(note => note.id === noteId) || null;

    return (
        <Box>
            {selectedNote ? (
                <Paper sx={{p: 3, flexGrow: 1}}>
                    <Typography variant="h4" gutterBottom>
                        {selectedNote.title}
                    </Typography>
                    <Typography variant="body1" whiteSpace="pre-wrap">
                        {selectedNote.content}
                    </Typography>
                </Paper>
            ) : (
                <Typography>Select a note</Typography>
            )}
        </Box>
    )
}
