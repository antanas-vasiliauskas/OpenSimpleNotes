package com.alpharoot.opensimplenotes.ui.viewmodels

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.alpharoot.opensimplenotes.data.models.NoteResponse
import com.alpharoot.opensimplenotes.data.repository.NotesRepository
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class NotesUiState(
    val notes: List<NoteResponse> = emptyList(),
    val isLoading: Boolean = false,
    val error: String? = null,
    val searchQuery: String = ""
)

data class NoteEditUiState(
    val note: NoteResponse? = null,
    val isLoading: Boolean = false,
    val error: String? = null,
    val isSaving: Boolean = false,
    val title: String = "",
    val content: String = "",
    val isPinned: Boolean = false
)

class NotesViewModel(private val notesRepository: NotesRepository) : ViewModel() {

    private val _uiState = MutableStateFlow(NotesUiState())
    val uiState: StateFlow<NotesUiState> = _uiState.asStateFlow()

    private val _editUiState = MutableStateFlow(NoteEditUiState())
    val editUiState: StateFlow<NoteEditUiState> = _editUiState.asStateFlow()

    private var autoSaveJob: Job? = null

    init {
        loadNotes()
    }

    // Add method to handle user changes
    fun onUserChanged(userId: String) {
        viewModelScope.launch {
            notesRepository.refreshNotesForNewUser(userId)
            // Clear current UI state and reload
            _uiState.value = NotesUiState()
            _editUiState.value = NoteEditUiState()
            loadNotes()
        }
    }

    // Add method to clear cache
    fun clearNotesCache() {
        notesRepository.clearCache()
        _uiState.value = NotesUiState()
        _editUiState.value = NoteEditUiState()
    }

    fun loadNotes() {
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(isLoading = true, error = null)

            notesRepository.getAllNotes()
                .onSuccess { notes ->
                    _uiState.value = _uiState.value.copy(
                        notes = notes.sortedWith(
                            compareByDescending<NoteResponse> { it.isPinned }
                                .thenByDescending { it.updatedAt }
                        ),
                        isLoading = false,
                        error = null
                    )
                }
                .onFailure { exception ->
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = exception.message ?: "Failed to load notes"
                    )
                }
        }
    }

    fun searchNotes(query: String) {
        _uiState.value = _uiState.value.copy(searchQuery = query)
    }

    fun getFilteredNotes(): List<NoteResponse> {
        val query = _uiState.value.searchQuery.lowercase()
        return if (query.isEmpty()) {
            _uiState.value.notes
        } else {
            _uiState.value.notes.filter { note ->
                note.title.lowercase().contains(query) ||
                note.content.lowercase().contains(query)
            }
        }
    }

    fun loadNoteForEdit(noteId: String) {
        viewModelScope.launch {
            _editUiState.value = _editUiState.value.copy(isLoading = true, error = null)

            notesRepository.getNoteById(noteId)
                .onSuccess { note ->
                    _editUiState.value = _editUiState.value.copy(
                        note = note,
                        title = note.title,
                        content = note.content,
                        isPinned = note.isPinned,
                        isLoading = false,
                        error = null
                    )
                }
                .onFailure { exception ->
                    _editUiState.value = _editUiState.value.copy(
                        isLoading = false,
                        error = exception.message ?: "Failed to load note"
                    )
                }
        }
    }

    fun createNewNote() {
        _editUiState.value = NoteEditUiState(
            title = "",
            content = "",
            isPinned = false
        )
    }

    fun updateTitle(title: String) {
        _editUiState.value = _editUiState.value.copy(title = title)
        scheduleAutoSave()
    }

    fun updateContent(content: String) {
        _editUiState.value = _editUiState.value.copy(content = content)
        scheduleAutoSave()
    }

    fun togglePin() {
        _editUiState.value = _editUiState.value.copy(
            isPinned = !_editUiState.value.isPinned
        )
        scheduleAutoSave()
    }

    private fun scheduleAutoSave() {
        autoSaveJob?.cancel()
        autoSaveJob = viewModelScope.launch {
            delay(2000) // 2 seconds debounce
            saveNote()
        }
    }

    fun saveNote() {
        viewModelScope.launch {
            val currentState = _editUiState.value
            if (currentState.title.isBlank() && currentState.content.isBlank()) return@launch

            _editUiState.value = currentState.copy(isSaving = true, error = null)

            val result = if (currentState.note == null) {
                // Create new note
                notesRepository.createNote(
                    title = currentState.title.ifBlank { "Untitled" },
                    content = currentState.content,
                    isPinned = currentState.isPinned
                )
            } else {
                // Update existing note
                notesRepository.updateNote(
                    id = currentState.note.id,
                    title = currentState.title.ifBlank { "Untitled" },
                    content = currentState.content,
                    isPinned = currentState.isPinned
                )
            }

            result
                .onSuccess { note ->
                    _editUiState.value = _editUiState.value.copy(
                        note = note,
                        isSaving = false,
                        error = null
                    )
                    loadNotes() // Refresh the notes list
                }
                .onFailure { exception ->
                    _editUiState.value = _editUiState.value.copy(
                        isSaving = false,
                        error = exception.message ?: "Failed to save note"
                    )
                }
        }
    }

    fun deleteNote(noteId: String) {
        viewModelScope.launch {
            notesRepository.deleteNote(noteId)
                .onSuccess {
                    loadNotes() // Refresh the notes list
                }
                .onFailure { exception ->
                    _uiState.value = _uiState.value.copy(
                        error = exception.message ?: "Failed to delete note"
                    )
                }
        }
    }

    fun clearError() {
        _uiState.value = _uiState.value.copy(error = null)
        _editUiState.value = _editUiState.value.copy(error = null)
    }

    override fun onCleared() {
        super.onCleared()
        autoSaveJob?.cancel()
    }
}
