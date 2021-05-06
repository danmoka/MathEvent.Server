import api from "../api";
import { baseService } from "./base-service";

const userService = {
    fetchUsers: async () => {
        const url = api.users.fetchUsers();

        return await baseService.get(url);
    },
    register: async (credentials) => {
        const url = api.users.registerUrl();

        return await baseService.post(url, credentials);
    },
    fetchStatistics: async (activeUsersTop) => {
        const url = api.users.fetchStatistics(activeUsersTop);

        return await baseService.get(url);
    },
};

export default userService;