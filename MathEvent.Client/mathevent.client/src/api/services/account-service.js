import api from "../api";
import { accountBaseService, baseService } from "./base-service";

const accountService = {
    token: async (data) => {
        const url = api.account.tokenUrl();

        return await accountBaseService.post(url, data);
    },
    userInfo: async () => {
        const url = api.account.infoUrl();

        return await baseService.get(url);
    },
    revocation: async (data) => {
        const url = api.account.revocationUrl();

        return await accountBaseService.post(url, data);
    },
};

export default accountService;