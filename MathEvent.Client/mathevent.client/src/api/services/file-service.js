import api from "../api";
import { baseService } from "./base-service";
import { getAccessToken } from "../../utils/local-storage-manager";

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
    createFile: async (createdFile) => {
        const url = api.files.createFile();

        return await baseService.post(url, createdFile);
    },
    deleteFile: async (fileId) => {
        const url = api.files.deleteFile(fileId);

        return await baseService.delete(url);
    },
    uploadFiles: async (fileId, ownerId, files) => {
        const url = api.files.uploadFiles(fileId, ownerId);

        try {
            const formData = new FormData();
            files.map((file) => {
                formData.append('files', file, file.name);
            });

            return await fetch(url, {
                method: "POST",
                body: formData,
                headers: {
                    Authorization: `Bearer ${getAccessToken()}`,
                },
            });
        } catch (e) {
            console.log(e);
        }
    },
    downloadFile: async (fileId) => {
        const url = api.files.downloadFile(fileId);

        return await baseService.get(url);
    },
};

export default fileService;