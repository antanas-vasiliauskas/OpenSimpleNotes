import { useEffect, useState } from "react";
import api from "../api/axios"; // use this, not axios directly

type Note = {
  id: string;
  title: string;
  content: string;
  isPinned: boolean;
  createdAt: string;
  updatedAt: string;
};

export default function NotesPage() {
  const [notes, setNotes] = useState<Note[]>([]);
  const [selectedNoteId, setSelectedNoteId] = useState<string | null>(null);
  const [isSyncing, setIsSyncing] = useState(false);

  const selectedNote = notes.find((note) => note.id === selectedNoteId);

  useEffect(() => {
    api
      .get("/notes")
      .then((res) => {
        const data = Array.isArray(res.data) ? res.data : [];
        setNotes(data);
        console.log(data);
        if (data.length > 0) setSelectedNoteId(data[0].id);
      })
      .catch((err) => console.error("Failed to load notes", err));
  }, []);

  const updateNoteField = (field: keyof Note, value: string) => {
    if (!selectedNote) return;

    const updatedNote = { ...selectedNote, [field]: value };
    setNotes((prev) =>
      prev.map((note) => (note.id === updatedNote.id ? updatedNote : note))
    );

    setIsSyncing(true);
    api
      .put(`/notes/${updatedNote.id}`, updatedNote)
      .finally(() => setIsSyncing(false));
  };

  const createNote = () => {
    api
      .post("/notes", {})
      .then((res) => {
        const newNote = res.data;
        setNotes((prev) => [newNote, ...prev]);
        setSelectedNoteId(newNote.id);
      })
      .catch((err) => console.error("Failed to create note", err));
  };

  const deleteNote = () => {
    if (!selectedNote) return;

    api
      .delete(`/notes/${selectedNote.id}`)
      .then(() => {
        setNotes((prev) => prev.filter((n) => n.id !== selectedNote.id));
        setSelectedNoteId(null);
      })
      .catch((err) => console.error("Failed to delete note", err));
  };

  return (
    <div style={{ display: "flex", height: "100vh" }}>
      <div style={{ width: 250, borderRight: "1px solid #ccc", padding: 10 }}>
        <button onClick={createNote}>New Note</button>
        <ul style={{ listStyle: "none", padding: 0 }}>
          {notes.map((note) => (
            <li
              key={note.id}
              style={{
                padding: 5,
                cursor: "pointer",
                background: note.id === selectedNoteId ? "#eee" : "transparent",
              }}
              onClick={() => setSelectedNoteId(note.id)}
            >
              {note.title || "Untitled"}
            </li>
          ))}
        </ul>
      </div>

      <div style={{ flex: 1, padding: 10 }}>
        {selectedNote ? (
          <>
            <input
              type="text"
              value={selectedNote.title}
              onChange={(e) => updateNoteField("title", e.target.value)}
              placeholder="Title"
              style={{ width: "100%", fontSize: 18, marginBottom: 10 }}
            />
            <textarea
              value={selectedNote.content}
              onChange={(e) => updateNoteField("content", e.target.value)}
              placeholder="Content"
              style={{ width: "100%", height: "70vh", fontSize: 16 }}
            />
            <br />
            <button onClick={deleteNote} style={{ marginTop: 10 }}>
              Delete Note
            </button>
          </>
        ) : (
          <p>Select or create a note.</p>
        )}
      </div>

      {isSyncing && (
        <div
          style={{
            position: "fixed",
            bottom: 20,
            right: 20,
            backgroundColor: "#eee",
            padding: "10px 20px",
            borderRadius: 4,
            boxShadow: "0 2px 6px rgba(0,0,0,0.2)",
          }}
        >
          Saving...
        </div>
      )}
    </div>
  );
}
