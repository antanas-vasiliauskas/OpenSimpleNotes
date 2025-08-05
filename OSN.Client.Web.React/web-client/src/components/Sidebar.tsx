import { useState } from 'react';
import { 
    Box, 
    Button, 
    List, 
    ListItem, 
    ListItemText, 
    IconButton, 
    Menu, 
    MenuItem, 
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Typography,
    styled,
    Avatar,
    ListItemIcon,
    ListItemButton
} from '@mui/material';
import { 
    Add as AddIcon, 
    MoreVert as MoreVertIcon,
    AccountCircle as AccountCircleIcon,
    Logout as LogoutIcon 
} from '@mui/icons-material';
import { Note } from '../types/notes';
import { useNavigate, useParams } from 'react-router-dom';

const SidebarContainer = styled(Box)({
    width: '280px',
    height: '100vh',
    display: 'flex',
    flexDirection: 'column',
    borderRight: '1px solid rgba(0, 0, 0, 0.12)',
    flexShrink: 0, // Prevent sidebar from shrinking
    backgroundColor: '#f5f5f5'
});

const LogoPlaceholder = styled(Box)({
    height: '64px',
    display: 'flex',
    alignItems: 'center',
    padding: '0 16px',
});

const CreateButtonContainer = styled(Box)({
    padding: '16px',
    paddingTop: '8px',
});

const ProfileSection = styled(Box)({
    marginTop: 'auto',
    borderTop: '1px solid rgba(0, 0, 0, 0.08)',
});

const ScrollableList = styled(List)({
    overflowY: 'auto',
    flex: 1,
    paddingTop: 0,
});

const StyledListItem = styled(ListItem)(({ theme }) => ({
    position: 'relative',
    padding: '8px 16px',
    cursor: 'pointer',
    '&:hover': {
        backgroundColor: 'rgba(0, 0, 0, 0.04)',
        '& .more-button': {
            opacity: 1,
        },
    },
    '& .MuiListItemText-root': {
        margin: 0,
    },
    '& .MuiTypography-root': {
        whiteSpace: 'nowrap',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        paddingRight: '40px', // Space for the more button
        background: 'linear-gradient(90deg, rgba(245,245,245,0) 0%, #f5f5f5 100%)',
        backgroundClip: 'text',
        WebkitBackgroundClip: 'text',
    },
}));

const MoreButton = styled(IconButton)({
    position: 'absolute',
    right: '8px',
    top: '50%',
    transform: 'translateY(-50%)',
    opacity: 0,
    backgroundColor: '#f5f5f5',
    padding: '4px',
    '&:hover': {
        backgroundColor: '#e0e0e0',
    },
});

interface SidebarProps {
    notes: Note[];
    onCreateNote: () => void;
    onDeleteNote: (noteId: string) => void;
    onLogout: () => void;
}

export default function Sidebar({ notes, onCreateNote, onDeleteNote, onLogout }: SidebarProps) {
    const navigate = useNavigate();
    const { noteId } = useParams();
    const [menuAnchorEl, setMenuAnchorEl] = useState<null | HTMLElement>(null);
    const [profileMenuAnchorEl, setProfileMenuAnchorEl] = useState<null | HTMLElement>(null);
    const [selectedNote, setSelectedNote] = useState<Note | null>(null);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

    const handleNoteClick = (note: Note) => {
        navigate(`/${note.id}`);
    };

    const handleMoreClick = (event: React.MouseEvent<HTMLButtonElement>, note: Note) => {
        event.stopPropagation();
        setSelectedNote(note);
        setMenuAnchorEl(event.currentTarget);
    };

    const handleDeleteClick = () => {
        setMenuAnchorEl(null);
        setDeleteDialogOpen(true);
    };

    const handleConfirmDelete = () => {
        if (selectedNote) {
            onDeleteNote(selectedNote.id);
            setDeleteDialogOpen(false);
            if (selectedNote.id === noteId) {
                navigate('/');
            }
        }
    };

    return (
        <SidebarContainer>
            <LogoPlaceholder>
                <Typography variant="h6">Open Simple Notes</Typography>
            </LogoPlaceholder>
            
            <CreateButtonContainer>
                <Button
                    variant="contained"
                    fullWidth
                    startIcon={<AddIcon />}
                    onClick={onCreateNote}
                >
                    New Note
                </Button>
            </CreateButtonContainer>

            <ScrollableList>
                {notes.map((note) => (
                    <StyledListItem
                        key={note.id}
                        onClick={() => handleNoteClick(note)}
                        sx={{ 
                            backgroundColor: note.id === noteId ? 'rgba(0, 0, 0, 0.08)' : 'transparent'
                        }}
                    >
                        <ListItemText 
                            primary={note.title || "Untitled Note"}
                            sx={{
                                opacity: note.title ? 1 : 0.6
                            }}
                        />
                        <MoreButton
                            size="small"
                            className="more-button"
                            onClick={(e) => handleMoreClick(e, note)}
                        >
                            <MoreVertIcon fontSize="small" />
                        </MoreButton>
                    </StyledListItem>
                ))}
            </ScrollableList>

            <ProfileSection>
                <ListItemButton
                    onClick={(e) => setProfileMenuAnchorEl(e.currentTarget)}
                    sx={{ 
                        py: 1.5,
                        '&:hover': {
                            backgroundColor: 'rgba(0, 0, 0, 0.04)'
                        }
                    }}
                >
                    <ListItemIcon>
                        <Avatar sx={{ width: 32, height: 32 }}>
                            <AccountCircleIcon />
                        </Avatar>
                    </ListItemIcon>
                    <ListItemText primary="My Profile" />
                </ListItemButton>
            </ProfileSection>

            <Menu
                anchorEl={menuAnchorEl}
                open={Boolean(menuAnchorEl)}
                onClose={() => setMenuAnchorEl(null)}
            >
                <MenuItem onClick={handleDeleteClick}>Delete</MenuItem>
            </Menu>

            <Menu
                anchorEl={profileMenuAnchorEl}
                open={Boolean(profileMenuAnchorEl)}
                onClose={() => setProfileMenuAnchorEl(null)}
            >
                <MenuItem onClick={() => {
                    setProfileMenuAnchorEl(null);
                    onLogout();
                }}>
                    <ListItemIcon>
                        <LogoutIcon fontSize="small" />
                    </ListItemIcon>
                    <ListItemText>Logout</ListItemText>
                </MenuItem>
            </Menu>

            <Dialog
                open={deleteDialogOpen}
                onClose={() => setDeleteDialogOpen(false)}
            >
                <DialogTitle>Delete Note</DialogTitle>
                <DialogContent>
                    <Typography>
                        Are you sure you want to delete note "{selectedNote?.title}"?
                    </Typography>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
                    <Button onClick={handleConfirmDelete} color="error">Delete</Button>
                </DialogActions>
            </Dialog>
        </SidebarContainer>
    );
}
