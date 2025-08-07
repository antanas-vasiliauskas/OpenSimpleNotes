// Google OAuth2 popup with dedicated callback page
export const initGoogleAuth = (onSuccess: (token: string) => void) => {
  (window as any).__googleAuthCallback = onSuccess;
};

export const promptGoogleSignIn = () => {
  const clientId = process.env.REACT_APP_GOOGLE_CLIENT_ID;
  if (!clientId) {
    console.error('Google Client ID not configured');
    return;
  }

  // Use dedicated callback page that will close the popup
  const redirectUri = `${window.location.origin}/oauth-callback.html`;

  const params = new URLSearchParams({
    client_id: clientId,
    redirect_uri: redirectUri,
    response_type: 'code',
    scope: 'openid email profile',
    state: Math.random().toString(36).substring(2),
    prompt: 'select_account'
  });

  const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?${params}`;

  // Open popup immediately
  const popup = window.open(
    authUrl,
    'googleAuth',
    'width=500,height=600,toolbar=0,scrollbars=1,status=1,resizable=1'
  );

  if (!popup) {
    alert('Please allow popups for this website to sign in with Google');
    return;
  }

  // Listen for messages from the callback page
  const handleMessage = (event: MessageEvent) => {
    // Only accept messages from our own origin
    if (event.origin !== window.location.origin) {
      return;
    }

    if (event.data.type === 'GOOGLE_AUTH_SUCCESS') {
      const callback = (window as any).__googleAuthCallback;
      if (callback) {
        callback(event.data.code);
      }
      clearInterval(checkClosed);
      window.removeEventListener('message', handleMessage);
    } else if (event.data.type === 'GOOGLE_AUTH_ERROR') {
      console.error('Google Auth Error:', event.data.error);
      clearInterval(checkClosed);
      window.removeEventListener('message', handleMessage);
    }
  };

  window.addEventListener('message', handleMessage);

  // Check for popup closure
  const checkClosed = setInterval(() => {
    if (popup.closed) {
      clearInterval(checkClosed);
      window.removeEventListener('message', handleMessage);
    }
  }, 1000);

  // Clean up after 5 minutes
  setTimeout(() => {
    clearInterval(checkClosed);
    window.removeEventListener('message', handleMessage);
    if (!popup.closed) {
      popup.close();
    }
  }, 300000);
};
