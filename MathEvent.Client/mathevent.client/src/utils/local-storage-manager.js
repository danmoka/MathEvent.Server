const ACCESS_TOKEN = "ACCESS_TOKEN";
const REFRESH_TOKEN = "REFRESH_TOKEN";

export const getAccessToken = () => localStorage.getItem(ACCESS_TOKEN);
export const setAccessToken = (accessToken) => localStorage.setItem(ACCESS_TOKEN, accessToken);
export const clearAccessToken = () => localStorage.removeItem(ACCESS_TOKEN);
export const getRefreshToken = () => localStorage.getItem(REFRESH_TOKEN);
export const setRefreshToken = (refreshToken) => localStorage.setItem(REFRESH_TOKEN, refreshToken);
export const clearRefreshToken = () => localStorage.removeItem(REFRESH_TOKEN);