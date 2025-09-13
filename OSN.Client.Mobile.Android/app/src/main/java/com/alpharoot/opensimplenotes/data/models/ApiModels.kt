package com.alpharoot.opensimplenotes.data.models

import com.google.gson.annotations.SerializedName
import java.util.*

// Request Models
data class LoginCommand(
    val email: String,
    val password: String
)

data class RegisterCommand(
    val email: String,
    val password: String
)

data class VerifyEmailCommand(
    val email: String,
    val verificationCode: String
)

data class VerifyResendCommand(
    val email: String
)

data class GoogleSignInCommand(
    val authorizationCode: String,
    val redirectUri: String
)

data class AnonymousLoginCommand(
    val guestId: String? = null
)

data class CreateNoteCommand(
    val title: String,
    val content: String,
    val isPinned: Boolean = false
)

data class UpdateNoteCommand(
    val id: String,
    val title: String,
    val content: String,
    val isPinned: Boolean = false
)

// Response Models
data class LoginResponse(
    val token: String,
    val role: String
)

data class RegisterResponse(
    val message: String
)

data class VerifyEmailResponse(
    val token: String,
    val role: String
)

data class VerifyResendResponse(
    val message: String
)

data class GoogleSignInResponse(
    val token: String,
    val role: String,
    val isNewUser: Boolean
)

data class AnonymousLoginResponse(
    val token: String,
    val role: String,
    val guestId: String,
    val isNewUser: Boolean
)

data class NoteResponse(
    val id: String,
    val title: String,
    val content: String,
    val isPinned: Boolean,
    val createdAt: String,
    val updatedAt: String
)

// Error Response
data class ErrorResponse(
    val message: String
)
