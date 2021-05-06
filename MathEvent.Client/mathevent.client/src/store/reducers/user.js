import { createSlice } from "@reduxjs/toolkit";
import {
    onPendingUsers,
    onFulfilledUsers,
    onRejectedUsers,
    onPendingUserStatistics,
    onFulfilledUserStatistics,
    onRejectedUserStatistics
} from "./defaults";
import { fetchUsers, fetchStatistics } from "../actions/user";

const initialState = {
    users: [],
    statistics: [],
    isFetchingUsers: false,
    isFetchingUserStatistics: false,
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

        [fetchStatistics.pending]: (state) => {
            onPendingUserStatistics(state);
        },
        [fetchStatistics.fulfilled]: (state, { payload: { statistics, hasError } }) => {
            onFulfilledUserStatistics(state, hasError);

            if (!hasError) {
                state.statistics = statistics;
            }
        },
        [fetchStatistics.rejected]: (state) => {
            onRejectedUserStatistics(state);
            state.statistics = [];
        },
    }
});

export default userSlice.reducer;