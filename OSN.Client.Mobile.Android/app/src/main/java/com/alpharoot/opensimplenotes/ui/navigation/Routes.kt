package com.alpharoot.opensimplenotes.ui.navigation

object Routes {
    const val LOGIN = "login"
    const val REGISTER = "register"
    const val VERIFY_EMAIL = "verify_email/{email}"
    const val NOTES_LIST = "notes_list"
    const val NOTE_EDIT = "note_edit?noteId={noteId}"

    fun verifyEmail(email: String) = "verify_email/$email"
    fun noteEdit(noteId: String? = null) = if (noteId != null) "note_edit?noteId=$noteId" else "note_edit"
}
