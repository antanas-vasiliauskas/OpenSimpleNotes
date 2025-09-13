package com.alpharoot.opensimplenotes.data.auth

import android.content.Context
import android.content.Intent
import android.util.Log
import com.alpharoot.opensimplenotes.R
import com.google.android.gms.auth.api.signin.GoogleSignIn
import com.google.android.gms.auth.api.signin.GoogleSignInClient
import com.google.android.gms.auth.api.signin.GoogleSignInOptions
import com.google.android.gms.common.api.ApiException
import com.google.android.gms.tasks.Task
import kotlinx.coroutines.suspendCancellableCoroutine
import kotlin.coroutines.resume
import kotlin.coroutines.resumeWithException

data class GoogleSignInResult(
    val authorizationCode: String,
    val redirectUri: String
)

class GoogleSignInManager(private val context: Context) {

    companion object {
        private const val TAG = "GoogleSignInManager"
        private const val REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob" // Standard OAuth2 redirect URI for mobile apps
    }

    private val googleSignInClient: GoogleSignInClient by lazy {
        Log.d(TAG, "Initializing Google Sign-In client for authorization code flow")
        val clientId = context.getString(R.string.google_client_id)
        Log.d(TAG, "Using Google Client ID: $clientId")

        val gso = GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
            .requestServerAuthCode(clientId) // Request authorization code instead of ID token
            .requestEmail()
            .build()

        GoogleSignIn.getClient(context, gso)
    }

    fun getSignInIntent(): Intent = googleSignInClient.signInIntent.also {
        Log.d(TAG, "Created Google Sign-In intent for authorization code")
    }

    fun getSignInIntentWithAccountPicker(): Intent {
        Log.d(TAG, "Creating Google Sign-In intent with account picker for authorization code")
        // Sign out first to ensure account picker is shown
        googleSignInClient.signOut()
        return googleSignInClient.signInIntent.also {
            Log.d(TAG, "Created Google Sign-In intent with forced account picker for authorization code")
        }
    }

    suspend fun handleSignInResult(data: Intent?): Result<GoogleSignInResult> {
        Log.d(TAG, "Handling Google Sign-In result for authorization code")
        Log.d(TAG, "Intent data is null: ${data == null}")

        return try {
            val task = GoogleSignIn.getSignedInAccountFromIntent(data)
            Log.d(TAG, "Got sign-in task from intent")

            val account = task.await()
            Log.d(TAG, "Successfully got account from task")
            Log.d(TAG, "Account email: ${account?.email}")
            Log.d(TAG, "Account display name: ${account?.displayName}")
            Log.d(TAG, "Account ID: ${account?.id}")

            val authCode = account?.serverAuthCode
            Log.d(TAG, "Authorization code is null: ${authCode == null}")

            if (authCode != null) {
                Log.d(TAG, "Authorization code length: ${authCode.length}")
                Log.d(TAG, "Authorization code preview: ${authCode.take(20)}...")
                Log.d(TAG, "Using redirect URI: $REDIRECT_URI")

                Result.success(
                    GoogleSignInResult(
                        authorizationCode = authCode,
                        redirectUri = REDIRECT_URI
                    )
                )
            } else {
                Log.e(TAG, "Failed to get authorization code from Google account")
                Result.failure(Exception("Failed to get authorization code"))
            }
        } catch (e: ApiException) {
            Log.e(TAG, "ApiException during sign-in: ${e.statusCode} - ${e.message}", e)
            Result.failure(e)
        } catch (e: Exception) {
            Log.e(TAG, "Exception during sign-in: ${e.message}", e)
            Result.failure(e)
        }
    }

    private suspend fun <T> Task<T>.await(): T = suspendCancellableCoroutine { continuation ->
        addOnCompleteListener { task ->
            if (task.exception != null) {
                continuation.resumeWithException(task.exception!!)
            } else {
                continuation.resume(task.result)
            }
        }
    }

    fun signOut() {
        Log.d(TAG, "Signing out from Google")
        googleSignInClient.signOut()
    }

    fun revokeAccess() {
        Log.d(TAG, "Revoking Google access")
        googleSignInClient.revokeAccess()
    }

    fun isSignedIn(): Boolean {
        val account = GoogleSignIn.getLastSignedInAccount(context)
        val isSignedIn = account != null
        Log.d(TAG, "Is signed in: $isSignedIn")
        return isSignedIn
    }
}
