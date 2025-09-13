package com.alpharoot.opensimplenotes.ui.navigation

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.runtime.*
import androidx.compose.ui.platform.LocalContext
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.alpharoot.opensimplenotes.data.api.ApiClient
import com.alpharoot.opensimplenotes.data.auth.GoogleSignInManager
import com.alpharoot.opensimplenotes.data.local.AuthTokenManager
import com.alpharoot.opensimplenotes.data.repository.AuthRepository
import com.alpharoot.opensimplenotes.data.repository.NotesRepository
import com.alpharoot.opensimplenotes.ui.screens.*
import com.alpharoot.opensimplenotes.ui.viewmodels.AuthViewModel
import com.alpharoot.opensimplenotes.ui.viewmodels.NotesViewModel

@Composable
fun OpenSimpleNotesNavigation(
    navController: NavHostController = rememberNavController()
) {
    val context = LocalContext.current

    // Initialize dependencies
    val tokenManager = remember { AuthTokenManager(context) }
    val apiService = remember { ApiClient.createApiService(tokenManager) }
    val authRepository = remember { AuthRepository(apiService, tokenManager) }
    val notesRepository = remember { NotesRepository(apiService) }
    val googleSignInManager = remember { GoogleSignInManager(context) }

    // ViewModels
    val authViewModel: AuthViewModel = viewModel { AuthViewModel(authRepository, googleSignInManager) }
    val notesViewModel: NotesViewModel = viewModel { NotesViewModel(notesRepository) }

    NavHost(
        navController = navController,
        startDestination = Routes.LOGIN
    ) {
        composable(
            Routes.LOGIN,
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            popEnterTransition = { EnterTransition.None },
            popExitTransition = { ExitTransition.None }
        ) {
            LoginScreen(
                authViewModel = authViewModel,
                onNavigateToRegister = {
                    navController.navigate(Routes.REGISTER)
                },
                onNavigateToNotes = {
                    val uiState = authViewModel.uiState.value
                    val userId = uiState.userEmail ?: uiState.guestId ?: uiState.userRole ?: "unknown"
                    notesViewModel.onUserChanged(userId)
                    navController.navigate(Routes.NOTES_LIST) {
                        popUpTo(Routes.LOGIN) { inclusive = true }
                    }
                },
                onGoogleSignIn = {
                    // Google Sign-In is handled internally by LoginScreen
                }
            )
        }

        composable(
            Routes.REGISTER,
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            popEnterTransition = { EnterTransition.None },
            popExitTransition = { ExitTransition.None }
        ) {
            RegisterScreen(
                authViewModel = authViewModel,
                onNavigateToLogin = {
                    navController.popBackStack()
                },
                onNavigateToVerify = { email ->
                    navController.navigate("verify_email/$email")
                }
            )
        }

        composable(
            "verify_email/{email}",
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            popEnterTransition = { EnterTransition.None },
            popExitTransition = { ExitTransition.None }
        ) { backStackEntry ->
            val email = backStackEntry.arguments?.getString("email") ?: ""
            VerifyEmailScreen(
                email = email,
                authViewModel = authViewModel,
                onNavigateToNotes = {
                    val uiState = authViewModel.uiState.value
                    val userId = uiState.userEmail ?: uiState.guestId ?: email
                    notesViewModel.onUserChanged(userId)
                    navController.navigate(Routes.NOTES_LIST) {
                        popUpTo(Routes.LOGIN) { inclusive = true }
                    }
                },
                onNavigateBack = {
                    navController.popBackStack()
                }
            )
        }

        composable(
            Routes.NOTES_LIST,
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            popEnterTransition = { EnterTransition.None },
            popExitTransition = { ExitTransition.None }
        ) {
            NotesListScreen(
                notesViewModel = notesViewModel,
                authViewModel = authViewModel,
                onNavigateToEdit = { noteId ->
                    if (noteId != null) {
                        navController.navigate("note_edit?noteId=$noteId")
                    } else {
                        navController.navigate("note_edit")
                    }
                },
                onNavigateToLogin = {
                    notesViewModel.clearNotesCache()
                    navController.navigate(Routes.LOGIN) {
                        popUpTo(Routes.NOTES_LIST) { inclusive = true }
                    }
                }
            )
        }

        composable(
            "note_edit?noteId={noteId}",
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            popEnterTransition = { EnterTransition.None },
            popExitTransition = { ExitTransition.None }
        ) { backStackEntry ->
            val noteId = backStackEntry.arguments?.getString("noteId")
            NoteEditScreen(
                noteId = noteId,
                notesViewModel = notesViewModel,
                onNavigateBack = {
                    navController.popBackStack()
                }
            )
        }

        composable(
            "note_edit",
            enterTransition = { EnterTransition.None },
            exitTransition = { ExitTransition.None },
            popEnterTransition = { EnterTransition.None },
            popExitTransition = { ExitTransition.None }
        ) {
            NoteEditScreen(
                noteId = null,
                notesViewModel = notesViewModel,
                onNavigateBack = {
                    navController.popBackStack()
                }
            )
        }
    }
}
