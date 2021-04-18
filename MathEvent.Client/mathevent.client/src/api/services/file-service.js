import api from "../api";
import baseService from "./base-service";

const fileService = {
    fetchFiles: async (fileId, ownerId) => {
        const url = api.files.fetchFiles(fileId, ownerId);

        return await baseService.get(url);
    },
    fetchFile: async (fileId) => {
        const url = api.files.fetchFile(fileId);

        return await baseService.get(url);
    },
    fetchFileBreadcrumbs: async (fileId) => {
        const url = api.files.fetchFileBreadcrumbs(fileId);

        return await baseService.get(url);
    },
};

export default fileService;