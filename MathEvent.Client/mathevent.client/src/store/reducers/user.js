import { createSlice } from '@reduxjs/toolkit';
import {
  onPendingUsers,
  onFulfilledUsers,
  onRejectedUsers,
  onPendingUser,
  onFulfilledUser,
  onRejectedUser,
  onPendingUserStatistics,
  onFulfilledUserStatistics,
  onRejectedUserStatistics,
} from './defaults';
import {
  fetchUser,
  fetchUsers,
  patchUser,
  fetchStatistics,
} from '../actions/user';

const initialState = {
  users: [],
  userInfo: null,
  statistics: [],
  isFetchingUsers: false,
  isFetchingUser: false,
  isFetchingUserStatistics: false,
  hasError: false,
};

const userSlice = createSlice({
  name: 'userSlice',
  initialState,
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

    [fetchUser.pending]: (state) => {
      onPendingUser(state);
    },
    [fetchUser.fulfilled]: (state, { payload: { user, hasError } }) => {
      onFulfilledUser(state, hasError);

      if (!hasError) {
        state.userInfo = user;
      }
    },
    [fetchUser.rejected]: (state) => {
      onRejectedUser(state);
      state.user = null;
    },

    [patchUser.pending]: (state) => {
      onPendingUser(state);
    },
    [patchUser.fulfilled]: (state, { payload: { updatedUser, hasError } }) => {
      onFulfilledUser(state, hasError);

      if (!hasError) {
        state.userInfo = updatedUser;
      }
    },
    [patchUser.rejected]: (state) => {
      onRejectedUser(state);
      state.user = null;
    },

    [fetchStatistics.pending]: (state) => {
      onPendingUserStatistics(state);
    },
    [fetchStatistics.fulfilled]: (
      state,
      { payload: { statistics, hasError } }
    ) => {
      onFulfilledUserStatistics(state, hasError);

      if (!hasError) {
        state.statistics = statistics;
      }
    },
    [fetchStatistics.rejected]: (state) => {
      onRejectedUserStatistics(state);
      state.statistics = [];
    },
  },
});

export default userSlice.reducer;
