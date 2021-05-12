import { createAsyncThunk } from '@reduxjs/toolkit';
import { navigateToLogin } from '../../utils/navigator';
import { showModal } from './modal';
import modalTypes from '../../constants/modal-types';
import userService from '../../api/services/user-service';
import statusCode from '../../utils/status-code-reader';

export const fetchUsers = createAsyncThunk('fecthUsers', async () => {
  const response = await userService.fetchUsers();

  if (statusCode(response).ok) {
    const users = await response.json();

    return { users };
  }

  return { users: [] };
});

export const fetchUser = createAsyncThunk('fecthUser', async (userId) => {
  const response = await userService.fetchUser(userId);

  if (statusCode(response).ok) {
    const user = await response.json();

    return { user };
  }

  return { users: null };
});

export const patchUser = createAsyncThunk(
  'patchUser',
  async ({ userId, data }) => {
    const response = await userService.patchUser(userId, data);

    if (statusCode(response).ok) {
      const updatedUser = await response.json();

      return { updatedUser, hasError: false };
    }

    return { hasError: true };
  }
);

export const register = createAsyncThunk('register', async (credentials) => {
  const response = await userService.register(credentials);

  if (statusCode(response).created) {
    navigateToLogin();
  }
});

export const fetchStatistics = createAsyncThunk(
  'fetchStatistics',
  async (activeUsersTop) => {
    const response = await userService.fetchStatistics(activeUsersTop);

    if (statusCode(response).ok) {
      const statistics = await response.json();

      return { statistics };
    }

    return { statistics: [] };
  }
);

export const fetchUserStatistics = createAsyncThunk(
  'fetchUserStatistics',
  async (userId) => {
    const response = await userService.fetchUserStatistics(userId);

    if (statusCode(response).ok) {
      const statistics = await response.json();

      return { statistics };
    }

    return { statistics: [] };
  }
);

export const showUserStatistics = createAsyncThunk(
  'showUserStatistics',
  async ({ user }, thunkAPI) => {
    thunkAPI.dispatch(showModal(modalTypes.userStatistics, { user }));
  }
);
