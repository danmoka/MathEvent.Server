import api from "../api";
import { baseService } from "./base-service";

const userService = {
    fetchUsers: async () => {
        const url = api.users.fetchUsers();

        return await baseService.get(url);
    },
};

export default userService;