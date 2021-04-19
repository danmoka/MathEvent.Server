import { createSlice } from "@reduxjs/toolkit";
import { onPendingAccount, onFulfilledAccount, onRejectedAccount } from "./defaults";
import { fetchTokens, fetchUserInfo, logout, revocation } from "../actions/account";
import { getAccessToken } from "../../utils/local-storage-manager";

const initialState = {
    userInfo: {},
    hasToken: Boolean(getAccessToken()),
    isAuthenticated: false,
    isFetchingAccount: false,
    hasError: false
};

const accountSlice = createSlice({
    name: "accountSlice",
    initialState: initialState,
    extraReducers: {
        [fetchTokens.pending]: (state) => {
            onPendingAccount(state);
        },
        [fetchTokens.fulfilled]: (state, { payload: { hasToken, hasError } }) => {
            onFulfilledAccount(state, hasError);

            if (!hasError) {
                state.hasToken = hasToken;
            }
        },
        [fetchTokens.rejected]: (state) => {
            onRejectedAccount(state);
            state.hasToken = false;
        },

        [fetchUserInfo.pending]: (state) => {
            onPendingAccount(state);
        },
        [fetchUserInfo.fulfilled]: (state, { payload: { userInfo, isAuthenticated, hasError } }) => {
            onFulfilledAccount(state, hasError);

            if (!hasError) {
                state.userInfo = userInfo;
                state.isAuthenticated = isAuthenticated;
            }
        },
        [fetchUserInfo.rejected]: (state) => {
            onRejectedAccount(state);
            state.userInfo = {};
            state.isAuthenticated = false;
        },

        [logout.fulfilled]: (state) => {
            onFulfilledAccount(state);
            state.userInfo = {};
            state.hasToken = false;
            state.isAuthenticated = false;
        },

        [revocation.pending]: (state) => {
            onPendingAccount(state);
        },
        [revocation.fulfilled]: (state, { payload: { hasError } }) => {
            onFulfilledAccount(state, hasError);
        },
        [revocation.rejected]: (state) => {
            onRejectedAccount(state);
        },
    }
});

export default accountSlice.reducer;