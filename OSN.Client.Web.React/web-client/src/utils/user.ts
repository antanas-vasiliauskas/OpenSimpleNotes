export const getUserRole = (): string | null => {
    return localStorage.getItem('userRole');
};

export const clearUserData = (): void => {
    localStorage.removeItem('userRole');
};
