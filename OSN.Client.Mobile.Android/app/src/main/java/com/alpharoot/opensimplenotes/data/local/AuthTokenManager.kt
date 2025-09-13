package com.alpharoot.opensimplenotes.data.local

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.map

val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "auth_preferences")

class AuthTokenManager(private val context: Context) {

    companion object {
        private val AUTH_TOKEN_KEY = stringPreferencesKey("auth_token")
        private val USER_ROLE_KEY = stringPreferencesKey("user_role")
        private val USER_EMAIL_KEY = stringPreferencesKey("user_email")
        private val GUEST_ID_KEY = stringPreferencesKey("guest_id")
    }

    fun getAuthToken(): Flow<String?> {
        return context.dataStore.data.map { preferences ->
            preferences[AUTH_TOKEN_KEY]
        }
    }

    fun getUserRole(): Flow<String?> {
        return context.dataStore.data.map { preferences ->
            preferences[USER_ROLE_KEY]
        }
    }

    fun getUserEmail(): Flow<String?> {
        return context.dataStore.data.map { preferences ->
            preferences[USER_EMAIL_KEY]
        }
    }

    fun getGuestId(): Flow<String?> {
        return context.dataStore.data.map { preferences ->
            preferences[GUEST_ID_KEY]
        }
    }

    suspend fun saveGuestId(guestId: String) {
        context.dataStore.edit { preferences ->
            preferences[GUEST_ID_KEY] = guestId
        }
    }

    suspend fun saveAuthData(token: String, role: String, email: String? = null, guestId: String? = null) {
        context.dataStore.edit { preferences ->
            preferences[AUTH_TOKEN_KEY] = token
            preferences[USER_ROLE_KEY] = role
            email?.let { preferences[USER_EMAIL_KEY] = it }
            guestId?.let { preferences[GUEST_ID_KEY] = it }
        }
    }

    suspend fun clearAuthData() {
        context.dataStore.edit { preferences ->
            preferences.remove(AUTH_TOKEN_KEY)
            preferences.remove(USER_ROLE_KEY)
            preferences.remove(USER_EMAIL_KEY)
            // Keep guest ID so user can log back into same guest account
            // preferences.remove(GUEST_ID_KEY)
        }
    }

    suspend fun isLoggedIn(): Boolean {
        return getAuthToken().first()?.isNotEmpty() == true
    }
}
