package com.alpharoot.opensimplenotes.data.repository

import com.alpharoot.opensimplenotes.data.api.ApiService
import com.alpharoot.opensimplenotes.data.local.AuthTokenManager
import com.alpharoot.opensimplenotes.data.models.*
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import java.util.*

class AuthRepository(
    private val apiService: ApiService,
    private val tokenManager: AuthTokenManager
) {
    suspend fun login(email: String, password: String): Result<LoginResponse> {
        return try {
            val response = apiService.login(LoginCommand(email, password))
            if (response.isSuccessful && response.body() != null) {
                val loginResponse = response.body()!!
                tokenManager.saveAuthData(loginResponse.token, loginResponse.role, email)
                Result.success(loginResponse)
            } else {
                Result.failure(Exception("Login failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun register(email: String, password: String): Result<RegisterResponse> {
        return try {
            val response = apiService.register(RegisterCommand(email, password))
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Registration failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun verifyEmail(email: String, code: String): Result<VerifyEmailResponse> {
        return try {
            val response = apiService.verifyEmail(VerifyEmailCommand(email, code))
            if (response.isSuccessful && response.body() != null) {
                val verifyResponse = response.body()!!
                tokenManager.saveAuthData(verifyResponse.token, verifyResponse.role, email)
                Result.success(verifyResponse)
            } else {
                Result.failure(Exception("Verification failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun resendVerification(email: String): Result<VerifyResendResponse> {
        return try {
            val response = apiService.verifyResend(VerifyResendCommand(email))
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Resend failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun googleSignIn(authorizationCode: String, redirectUri: String): Result<GoogleSignInResponse> {
        return try {
            val response = apiService.googleSignIn(GoogleSignInCommand(authorizationCode, redirectUri))
            if (response.isSuccessful && response.body() != null) {
                val googleResponse = response.body()!!
                tokenManager.saveAuthData(googleResponse.token, googleResponse.role)
                Result.success(googleResponse)
            } else {
                Result.failure(Exception("Google sign in failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun anonymousLogin(): Result<AnonymousLoginResponse> {
        return try {
            // Use stored guest ID if available, otherwise send null to create new guest
            val storedGuestId = tokenManager.getGuestId().first()

            val response = apiService.anonymousLogin(AnonymousLoginCommand(guestId = storedGuestId))
            if (response.isSuccessful && response.body() != null) {
                val anonymousResponse = response.body()!!
                tokenManager.saveAuthData(
                    token = anonymousResponse.token,
                    role = anonymousResponse.role,
                    guestId = anonymousResponse.guestId
                )
                Result.success(anonymousResponse)
            } else {
                Result.failure(Exception("Anonymous login failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun logout() {
        tokenManager.clearAuthData()
    }

    fun getAuthToken(): Flow<String?> = tokenManager.getAuthToken()
    fun getUserRole(): Flow<String?> = tokenManager.getUserRole()
    fun getUserEmail(): Flow<String?> = tokenManager.getUserEmail()
    fun getGuestId(): Flow<String?> = tokenManager.getGuestId()
}
