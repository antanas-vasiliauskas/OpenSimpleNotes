import { List, ListItem, ListItemText } from '@mui/material';
import { Note } from "../types/notes";
import { useNavigate } from 'react-router-dom';

export default function NoteList({notes}: {notes: Note[] }) {
    const navigate = useNavigate();
    return (
        <List sx={{ width: 250, bgcolor: 'background.paper'}}>
            {notes.map((note) => (
                <ListItem
                    key={note.id}
                    onClick={() => navigate(`/${note.id}`)}
                    sx={{
                        cursor: 'pointer',
                        '&:hover': {
                            backgroundColor: 'action.hover',
                        },
                    }}
                >
                    <ListItemText primary={note.title}/>
                </ListItem>
            ))}
        </List>
    );
}
