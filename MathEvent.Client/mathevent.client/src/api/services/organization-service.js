import api from "../api";
import baseService from "./base-service";

const organizationService = {
    fetchOrganizations: async () => {
        const url = api.organizations.fetchOrganizations();

        return await baseService.get(url);
    },
};

export default organizationService;