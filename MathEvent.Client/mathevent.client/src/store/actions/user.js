import { createAsyncThunk } from "@reduxjs/toolkit";
import { navigateToLogin } from "../../utils/navigator";
import userService from "../../api/services/user-service";
import statusCode from "../../utils/status-code-reader";

export const fetchUsers = createAsyncThunk("fecthUsers", async () => {
    const response = await userService.fetchUsers();

    if (statusCode(response).ok) {
        const users = await response.json();

        return { users };
    }

    return { users: [] };
});

export const register = createAsyncThunk("register", async (credentials) => {
    const response = await userService.register(credentials);

    if (statusCode(response).created) {
      navigateToLogin();
    }
});