package com.alpharoot.opensimplenotes.data.repository

import com.alpharoot.opensimplenotes.data.api.ApiService
import com.alpharoot.opensimplenotes.data.models.*

class NotesRepository(private val apiService: ApiService) {

    private var cachedNotes: List<NoteResponse>? = null
    private var currentUserId: String? = null

    fun clearCache() {
        cachedNotes = null
        currentUserId = null
    }

    fun setCurrentUser(userId: String) {
        if (currentUserId != userId) {
            clearCache()
            currentUserId = userId
        }
    }

    suspend fun getAllNotes(forceRefresh: Boolean = false): Result<List<NoteResponse>> {
        if (!forceRefresh && cachedNotes != null) {
            return Result.success(cachedNotes!!)
        }

        return try {
            val response = apiService.getAllNotes()
            if (response.isSuccessful && response.body() != null) {
                cachedNotes = response.body()!!
                Result.success(cachedNotes!!)
            } else {
                Result.failure(Exception("Failed to fetch notes"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    // Call this when user logs out or switches accounts
    suspend fun refreshNotesForNewUser(userId: String): Result<List<NoteResponse>> {
        setCurrentUser(userId)
        return getAllNotes(forceRefresh = true)
    }

    suspend fun getNoteById(id: String): Result<NoteResponse> {
        return try {
            val response = apiService.getNoteById(id)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Failed to fetch note"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun createNote(title: String, content: String, isPinned: Boolean = false): Result<NoteResponse> {
        return try {
            val response = apiService.createNote(CreateNoteCommand(title, content, isPinned))
            if (response.isSuccessful && response.body() != null) {
                // Clear cache to force refresh on next getAllNotes call
                cachedNotes = null
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Failed to create note"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun updateNote(id: String, title: String, content: String, isPinned: Boolean = false): Result<NoteResponse> {
        return try {
            val response = apiService.updateNote(UpdateNoteCommand(id, title, content, isPinned))
            if (response.isSuccessful && response.body() != null) {
                // Clear cache to force refresh on next getAllNotes call
                cachedNotes = null
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Failed to update note"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun deleteNote(id: String): Result<Unit> {
        return try {
            val response = apiService.deleteNote(id)
            if (response.isSuccessful) {
                // Clear cache to force refresh on next getAllNotes call
                cachedNotes = null
                Result.success(Unit)
            } else {
                Result.failure(Exception("Failed to delete note"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
