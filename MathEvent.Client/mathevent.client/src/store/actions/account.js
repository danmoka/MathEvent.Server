import { createAsyncThunk } from "@reduxjs/toolkit";
import { clearAccessToken, clearRefreshToken, getRefreshToken, setAccessToken, setRefreshToken } from "../../utils/local-storage-manager";
import accountService from "../../api/services/account-service";
import statusCode from "../../utils/status-code-reader";
import config from "../../config";

export const fetchTokens = createAsyncThunk("fetchTokens", async ({ userName, password }) => {
    let data = {
        client_id: config.clientId,
        client_secret: config.clientSecret
    };

    if (userName && password) {
        data = {...data,
            grant_type: config.passwordGrantType,
            userName: userName,
            password: password
        };
    }
    else {
        const refreshToken = getRefreshToken();
        data = {...data,
            grant_type: config.refreshGrantType,
            refresh_token: refreshToken
        };
    }

    const response = await accountService.token(data);

    if (statusCode(response).ok) {
        const { access_token, refresh_token } = await response.json();
        setAccessToken(access_token);
        setRefreshToken(refresh_token);

        return {
            hasToken: true,
            hasError: false
        };
    }

    return {
        hasToken: false,
        hasError: true
    };
});

export const fetchUserInfo = createAsyncThunk("fetchUserInfo", async () => {
    const response = await accountService.userInfo();

    if (statusCode(response).ok) {
        const userInfo = await response.json();

        return { userInfo, isAuthenticated: true };
    }

    return { userInfo: {}, isAuthenticated: false };
});

export const logout = createAsyncThunk("logout", () => {
    clearAccessToken();
    clearRefreshToken();
});

export const revocation = createAsyncThunk("revocation", async () => {
    const refreshToken = getRefreshToken();

    const data = {
        token: refreshToken,
        client_id: config.clientId,
        client_secret: config.clientSecret
    };

    const response = await accountService.revocation(data);

    if (statusCode(response).ok) {
        return { hasError: false };
    }

    return { hasError: true };
});