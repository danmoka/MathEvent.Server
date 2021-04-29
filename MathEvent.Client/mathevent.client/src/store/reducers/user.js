import { createSlice } from "@reduxjs/toolkit";
import {
    onPendingUsers,
    onFulfilledUsers,
    onRejectedUsers } from "./defaults";
import { fetchUsers } from "../actions/user";

const initialState = {
    users: [],
    isFetchingUsers: false,
    hasError: false,
}

const userSlice = createSlice({
    name: "userSlice",
    initialState: initialState,
    extraReducers: {
        [fetchUsers.pending]: (state) => {
            onPendingUsers(state);
        },
        [fetchUsers.fulfilled]: (state, { payload: { users, hasError } }) => {
            onFulfilledUsers(state, hasError);

            if (!hasError) {
                state.users = users;
            }
        },
        [fetchUsers.rejected]: (state) => {
            onRejectedUsers(state);
            state.users = [];
        },
    }
});

export default userSlice.reducer;