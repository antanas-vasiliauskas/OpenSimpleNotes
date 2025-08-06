declare global {
  interface Window {
    google?: {
      accounts: {
        id: {
          initialize: (config: { client_id: string; callback: (response: { credential: string }) => void }) => void;
          prompt: () => void;
        };
      };
    };
  }
}

export const initGoogleAuth = (onSuccess: (token: string) => void) => {
  const clientId = process.env.REACT_APP_GOOGLE_CLIENT_ID;
  if (!clientId || !window.google) return;

  window.google.accounts.id.initialize({
    client_id: clientId,
    callback: (response) => onSuccess(response.credential),
  });
};

export const promptGoogleSignIn = () => {
  if (window.google) {
    window.google.accounts.id.prompt();
  }
};
