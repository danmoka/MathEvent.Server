import { createAsyncThunk } from "@reduxjs/toolkit";
import fileService from "../../api/services/file-service";
import statusCode from "../../utils/status-code-reader";

export const fetchFiles = createAsyncThunk("fetchFiles", async ({fileId, ownerId}) => {
    const response = await fileService.fetchFiles(fileId, ownerId);

    if (statusCode(response).ok) {
        const files = await response.json();

        return { files };
    }

    return { files: [] };
});

export const fetchFile = createAsyncThunk("fetchFile", async (fileId) => {
    const response = await fileService.fetchFile(fileId);

    if (statusCode(response).ok) {
        const file = await response.json();

        return { file };
    }

    return { file: null };
});

export const fetchFileBreadcrumbs = createAsyncThunk("fetchFileBreadcrumbs", async (fileId) => {
    if (!fileId) {
        return { crumbs: [] };
    }

    const response = await fileService.fetchFileBreadcrumbs(fileId);

    if (statusCode(response).ok) {
        const crumbs = await response.json();

        return { crumbs };
    }

    return { crumbs: [] };
});