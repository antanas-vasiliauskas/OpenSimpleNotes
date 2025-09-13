package com.alpharoot.opensimplenotes.data.api

import com.alpharoot.opensimplenotes.data.models.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {

    @POST("api/auth/login")
    suspend fun login(@Body request: LoginCommand): Response<LoginResponse>

    @POST("api/auth/register")
    suspend fun register(@Body request: RegisterCommand): Response<RegisterResponse>

    @POST("api/auth/verify")
    suspend fun verifyEmail(@Body request: VerifyEmailCommand): Response<VerifyEmailResponse>

    @POST("api/auth/verify-resend")
    suspend fun verifyResend(@Body request: VerifyResendCommand): Response<VerifyResendResponse>

    @POST("api/auth/google-signin")
    suspend fun googleSignIn(@Body request: GoogleSignInCommand): Response<GoogleSignInResponse>

    @POST("api/auth/anonymous-login")
    suspend fun anonymousLogin(@Body request: AnonymousLoginCommand): Response<AnonymousLoginResponse>

    @GET("api/note")
    suspend fun getAllNotes(): Response<List<NoteResponse>>

    @GET("api/note/{id}")
    suspend fun getNoteById(@Path("id") id: String): Response<NoteResponse>

    @POST("api/note")
    suspend fun createNote(@Body request: CreateNoteCommand): Response<NoteResponse>

    @PUT("api/note")
    suspend fun updateNote(@Body request: UpdateNoteCommand): Response<NoteResponse>

    @DELETE("api/note/{id}")
    suspend fun deleteNote(@Path("id") id: String): Response<Unit>
}
