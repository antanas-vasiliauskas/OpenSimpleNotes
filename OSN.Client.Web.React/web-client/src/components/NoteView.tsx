import { useState, useEffect, useCallback, useRef } from 'react';
import { Box, styled, Typography } from '@mui/material';
import { useParams, useOutletContext } from 'react-router-dom';
import { Note } from '../types/notes';
import { useDebounce } from '../hooks/useDebounce';
import { Sync as SyncIcon, CheckCircle as CheckCircleIcon } from '@mui/icons-material';
import api from '../api/client';

interface NoteContextType {
    notes: Note[];
    onUpdateNote?: (noteId: string, updates: Partial<Note>) => void;
}

const NoteContainer = styled(Box)(({ theme }) => ({
    flex: 1,
    backgroundColor: '#ffffff',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    fontFamily: 'Inter, Roboto, system-ui, sans-serif',
    minHeight: '100%',
    position: 'relative',
}));

const ContentContainer = styled(Box)(({ theme }) => ({
    width: '60%', // Default width
    position: 'relative',
    flexGrow: 1,
    '@media (max-width: 1600px)': {
        width: 'calc(90% - 40px)', // Margins shrink with page
        maxWidth: '60%', // But content won't exceed 60%
    },
    '@media (max-width: 900px)': {
        width: '90%', // Minimum margins of 5% each side
    },
}));

const TitleContainer = styled(Box)({
    position: 'sticky',
    top: 0,
    backgroundColor: '#ffffff',
    padding: '40px 0 8px 0',
    width: '100%',
    zIndex: 1,
});

const TitleInput = styled('input')({
    width: '100%',
    border: 'none',
    outline: 'none',
    fontSize: '1.5rem',
    fontWeight: 'bold',
    fontFamily: 'inherit',
    marginBottom: '8px',
    background: 'transparent',
    padding: '0',
});

const TitleUnderline = styled(Box)({
    height: '1px',
    backgroundColor: '#000000',
    opacity: 0.1,
    width: '100%',
});

const ContentInput = styled('div')({
    width: '100%',
    border: 'none',
    outline: 'none',
    fontSize: '1rem',
    fontFamily: 'inherit',
    background: 'transparent',
    padding: '8px 0 40px 0',
    minHeight: '500px',
    whiteSpace: 'pre-wrap',
    wordWrap: 'break-word',
    wordBreak: 'break-word',
    overflowX: 'hidden',
    overflowWrap: 'break-word',
    position: 'relative',
    '&[contenteditable="true"]': {
        cursor: 'text',
        '-webkit-user-modify': 'read-write-plaintext-only',
        'user-modify': 'read-write-plaintext-only',
        '-moz-user-modify': 'read-write-plaintext-only',
        '-webkit-user-select': 'text',
        'user-select': 'text',
    },
    '& *': { // Remove any styling from pasted content
        color: 'inherit',
        backgroundColor: 'transparent',
        fontFamily: 'inherit',
        fontSize: 'inherit',
        textDecoration: 'inherit',
        fontWeight: 'inherit',
        fontStyle: 'inherit',
    },
    '&[data-empty="true"]::before': {
        content: '"Start writing..."',
        color: '#999',
        pointerEvents: 'none',
        position: 'absolute',
        opacity: 0.6,
        top: '8px',
        left: 0,
    },
    '&:focus': {
        spellcheck: false,
    },
});

const SyncStatus = styled(Box)({
    position: 'fixed',
    bottom: '16px',
    right: '16px',
    display: 'flex',
    alignItems: 'center',
    gap: '4px',
    color: '#999',
    fontSize: '0.75rem',
    '& svg': {
        width: '16px',
        height: '16px',
    },
});

type SyncState = 'syncing' | 'saved' | 'idle';

export default function NoteView() {
    const { noteId } = useParams();
    const { notes, onUpdateNote } = useOutletContext<NoteContextType>();
    const [title, setTitle] = useState('');
    const [syncState, setSyncState] = useState<SyncState>('idle');
    const currentNote = notes.find(note => note.id === noteId);

    const isLocalChange = useRef(false);

    const contentRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (currentNote && !isLocalChange.current) {
            setTitle(currentNote.title);
            if (contentRef.current) {
                // Replace newlines with <br> tags for proper display
                contentRef.current.innerHTML = currentNote.content.replace(/\n/g, '<br>');
                // Check if the loaded content is empty or whitespace-only
                const isOnlyWhitespace = currentNote.content.trim() === '';
                contentRef.current.setAttribute('data-empty', isOnlyWhitespace.toString());
            }
        }
        isLocalChange.current = false;
    }, [currentNote]);

    type SaveNoteFunction = (updates: Partial<Note>) => Promise<void>;

    const saveNote: SaveNoteFunction = useCallback(async (updates) => {
        if (!noteId) return;
        
        try {
            await api.put(`/note/${noteId}`, updates);
            isLocalChange.current = true;  // Mark this as a local change
            onUpdateNote?.(noteId, updates);
            setSyncState('saved');
            // Reset to idle after a short delay
            setTimeout(() => setSyncState('idle'), 2000);
        } catch (error) {
            console.error('Failed to save note:', error);
            setSyncState('idle');
        }
    }, [noteId, onUpdateNote]);

    const debouncedSave = useDebounce<SaveNoteFunction>(saveNote, 1000);

    const handleChange = useCallback((updates: Partial<Note>) => {
        if (updates.title !== undefined) setTitle(updates.title);
        setSyncState('syncing');
        debouncedSave(updates);
    }, [debouncedSave]);

    const handleTitleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            contentRef.current?.focus();
        }
    };

    const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        handleChange({ title: e.target.value });
    };

    const handleContentChange = (e: React.FormEvent<HTMLDivElement>) => {
        // Convert <br> and <div> elements to newlines for proper text handling
        const content = e.currentTarget.innerHTML
            .replace(/<br\s*\/?>/gi, '\n') // Replace <br> with newline
            .replace(/<div>(.*?)<\/div>/gi, '\n$1') // Replace <div>content</div> with newline + content
            .replace(/&nbsp;/g, ' ') // Replace &nbsp; with space
            .replace(/<[^>]*>/g, ''); // Remove any remaining HTML tags
            
        // Check if content is only whitespace
        const isOnlyWhitespace = content.trim() === '';
        e.currentTarget.setAttribute('data-empty', isOnlyWhitespace.toString());
        
        handleChange({ content });
    };

    const handlePaste = useCallback((e: React.ClipboardEvent<HTMLDivElement>) => {
        e.preventDefault();
        // Check if clipboard has anything other than text
        const hasNonText = Array.from(e.clipboardData.types).some(type => 
            !['text/plain', 'text/html'].includes(type)
        );

        if (hasNonText) {
            return; // Block paste if contains non-text content
        }

        // Only get plain text, ignore any HTML
        const text = e.clipboardData.getData('text/plain');
        if (text) {
            // Insert text at cursor position, preserving newlines
            const lines = text.split('\n');
            const processedText = lines.join('<br>');
            document.execCommand('insertHTML', false, processedText);
        }
    }, []);

    const handleDrop = useCallback((e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        return false;
    }, []);

    const handleDragOver = useCallback((e: React.DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        return false;
    }, []);

    const handleBlur = useCallback(() => {
        // Only flush if there are pending changes
        if (syncState === 'syncing') {
            debouncedSave.flush();
        }
    }, [debouncedSave, syncState]);

    useEffect(() => {
        return () => {
            if (syncState === 'syncing' && debouncedSave.flush) {
                debouncedSave.flush();
            }
        };
    }, [debouncedSave, syncState]);

    if (!currentNote) {
        return (
            <NoteContainer>
                <Typography>Select a note</Typography>
            </NoteContainer>
        );
    }

    return (
        <NoteContainer>
            <ContentContainer>
                <TitleContainer>
                    <TitleInput
                        value={title}
                        onChange={handleTitleChange}
                        onKeyDown={handleTitleKeyDown}
                        onBlur={handleBlur}
                        placeholder="Title"
                    />
                    <TitleUnderline />
                </TitleContainer>
                <ContentInput
                    ref={contentRef}
                    contentEditable
                    data-empty="true"
                    onInput={handleContentChange}
                    onBlur={handleBlur}
                    onPaste={handlePaste}
                    onDrop={handleDrop}
                    onDragOver={handleDragOver}
                    spellCheck={false}
                    />
            </ContentContainer>
            <SyncStatus>
                {syncState === 'syncing' && <SyncIcon />}
                {syncState === 'saved' && <CheckCircleIcon />}
            </SyncStatus>
        </NoteContainer>
    );
}
