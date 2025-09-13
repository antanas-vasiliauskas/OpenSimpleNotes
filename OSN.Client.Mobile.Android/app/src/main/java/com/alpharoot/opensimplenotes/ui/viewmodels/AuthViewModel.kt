package com.alpharoot.opensimplenotes.ui.viewmodels

import android.content.Intent
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.alpharoot.opensimplenotes.data.auth.GoogleSignInManager
import com.alpharoot.opensimplenotes.data.repository.AuthRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class AuthUiState(
    val isLoading: Boolean = false,
    val isLoggedIn: Boolean = false,
    val error: String? = null,
    val userEmail: String? = null,
    val userRole: String? = null,
    val guestId: String? = null
)

class AuthViewModel(
    private val authRepository: AuthRepository,
    private val googleSignInManager: GoogleSignInManager
) : ViewModel() {

    private val _uiState = MutableStateFlow(AuthUiState())
    val uiState: StateFlow<AuthUiState> = _uiState.asStateFlow()

    init {
        checkAuthStatus()
    }

    private fun checkAuthStatus() {
        viewModelScope.launch {
            authRepository.getAuthToken().collect { token ->
                _uiState.value = _uiState.value.copy(
                    isLoggedIn = !token.isNullOrEmpty()
                )
            }
        }

        viewModelScope.launch {
            authRepository.getUserEmail().collect { email ->
                _uiState.value = _uiState.value.copy(userEmail = email)
            }
        }

        viewModelScope.launch {
            authRepository.getUserRole().collect { role ->
                _uiState.value = _uiState.value.copy(userRole = role)
            }
        }

        viewModelScope.launch {
            authRepository.getGuestId().collect { guestId ->
                _uiState.value = _uiState.value.copy(guestId = guestId)
            }
        }
    }

    fun login(email: String, password: String) {
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(isLoading = true, error = null)

            authRepository.login(email, password)
                .onSuccess {
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        isLoggedIn = true,
                        error = null
                    )
                }
                .onFailure { exception ->
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = exception.message ?: "Login failed"
                    )
                }
        }
    }

    fun register(email: String, password: String) {
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(isLoading = true, error = null)

            authRepository.register(email, password)
                .onSuccess {
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = null
                    )
                }
                .onFailure { exception ->
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = exception.message ?: "Registration failed"
                    )
                }
        }
    }

    fun verifyEmail(email: String, code: String) {
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(isLoading = true, error = null)

            authRepository.verifyEmail(email, code)
                .onSuccess {
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        isLoggedIn = true,
                        error = null
                    )
                }
                .onFailure { exception ->
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = exception.message ?: "Verification failed"
                    )
                }
        }
    }

    fun resendVerification(email: String) {
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(isLoading = true, error = null)

            authRepository.resendVerification(email)
                .onSuccess {
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = null
                    )
                }
                .onFailure { exception ->
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = exception.message ?: "Resend failed"
                    )
                }
        }
    }

    fun getGoogleSignInIntent(): Intent {
        return googleSignInManager.getSignInIntentWithAccountPicker()
    }

    fun handleGoogleSignInResult(data: Intent?) {
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(isLoading = true, error = null)

            googleSignInManager.handleSignInResult(data)
                .onSuccess { googleSignInResult ->
                    // Use the authorization code and redirect URI to authenticate with backend
                    authRepository.googleSignIn(
                        authorizationCode = googleSignInResult.authorizationCode,
                        redirectUri = googleSignInResult.redirectUri
                    )
                        .onSuccess {
                            _uiState.value = _uiState.value.copy(
                                isLoading = false,
                                isLoggedIn = true,
                                error = null
                            )
                        }
                        .onFailure { exception ->
                            _uiState.value = _uiState.value.copy(
                                isLoading = false,
                                error = exception.message ?: "Google sign in failed"
                            )
                        }
                }
                .onFailure { exception ->
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = exception.message ?: "Google sign in failed"
                    )
                }
        }
    }

    fun anonymousLogin() {
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(isLoading = true, error = null)

            authRepository.anonymousLogin()
                .onSuccess {
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        isLoggedIn = true,
                        error = null
                    )
                }
                .onFailure { exception ->
                    _uiState.value = _uiState.value.copy(
                        isLoading = false,
                        error = exception.message ?: "Anonymous login failed"
                    )
                }
        }
    }

    fun logout() {
        viewModelScope.launch {
            authRepository.logout()
            googleSignInManager.signOut() // Also sign out from Google
            _uiState.value = AuthUiState() // Reset to initial state
        }
    }

    fun clearError() {
        _uiState.value = _uiState.value.copy(error = null)
    }
}
