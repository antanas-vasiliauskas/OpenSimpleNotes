interface JwtPayload {
    exp?: number;
    [key: string]: any;
}

export const isTokenExpired = (token: string): boolean => {
    try {
        // Get the expiration part from the token
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const payload: JwtPayload = JSON.parse(window.atob(base64));
        
        if (!payload.exp) return true;

        // exp is in seconds, Date.now() is in milliseconds
        const currentTime = Date.now() / 1000;
        return payload.exp < currentTime;
    } catch (error) {
        // If we can't decode the token, consider it invalid
        return true;
    }
};
