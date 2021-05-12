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
  onPendingUsersStatistics,
  onFulfilledUsersStatistics,
  onRejectedUsersStatistics,
} from './defaults';
import {
  fetchUser,
  fetchUsers,
  patchUser,
  fetchStatistics,
  fetchUserStatistics,
} from '../actions/user';

const initialState = {
  users: [],
  userInfo: null,
  statistics: [],
  userStatistics: [],
  isFetchingUser: false,
  isFetchingUsers: false,
  isFetchingUserStatistics: false,
  isFetchingUsersStatistics: false,
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
      onPendingUsersStatistics(state);
    },
    [fetchStatistics.fulfilled]: (
      state,
      { payload: { statistics, hasError } }
    ) => {
      onFulfilledUsersStatistics(state, hasError);

      if (!hasError) {
        state.statistics = statistics;
      }
    },
    [fetchStatistics.rejected]: (state) => {
      onRejectedUsersStatistics(state);
      state.statistics = [];
    },

    [fetchUserStatistics.pending]: (state) => {
      onPendingUserStatistics(state);
    },
    [fetchUserStatistics.fulfilled]: (
      state,
      { payload: { statistics, hasError } }
    ) => {
      onFulfilledUserStatistics(state, hasError);

      if (!hasError) {
        state.userStatistics = statistics;
      }
    },
    [fetchUserStatistics.rejected]: (state) => {
      onRejectedUserStatistics(state);
      state.statistics = [];
    },
  },
});

export default userSlice.reducer;
