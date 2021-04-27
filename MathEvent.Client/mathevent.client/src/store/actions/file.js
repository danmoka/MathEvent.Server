import { createAsyncThunk } from "@reduxjs/toolkit";
import { showModal, hideModal } from "./modal";
import fileService from "../../api/services/file-service";
import modalTypes from "../../constants/modal-types";
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

export const createFile = createAsyncThunk("createFile", async ({ file }, thunkAPI) => {
    thunkAPI.dispatch(hideModal());
    const response = await fileService.createFile(file);

    if (statusCode(response).created) {
        const createdFile = await response.json();
        thunkAPI.dispatch(fetchFiles({fileId: createdFile.parentId, ownerId: createdFile.ownerId}));

        return { createdFile, hasError: false };
    }

    return { hasError: true };
});

export const deleteFile = createAsyncThunk("deleteFile", async ({ fileId }, thunkAPI) => {
    thunkAPI.dispatch(hideModal());
    const response = await fileService.deleteFile(fileId);

    if (statusCode(response).noContent) {
        return { fileId, hasError: false };
    }

    return { hasError: true };
});

export const uploadFiles = createAsyncThunk(
    "uploadFiles",
    async ({ fileId, ownerId, files }, thunkAPI) => {
        thunkAPI.dispatch(hideModal());
        const response = await fileService.uploadFiles(fileId, ownerId, files);

        if (statusCode(response).ok) {
            thunkAPI.dispatch(fetchFiles({ fileId, ownerId }));

            return { hasError: false };
        }
    
        return { hasError: true };
    }
  );

export const showCreateFolderModal = createAsyncThunk("showCreateFolderModal", async ({ owner, crumbs }, thunkAPI) => {
    thunkAPI.dispatch(showModal(modalTypes.createFolder, { owner, crumbs }));
});
export const showDeleteFileModal = createAsyncThunk("showDeleteFileModal", async ({ file }, thunkAPI) => {
    thunkAPI.dispatch(showModal(modalTypes.deleteFile, { file }));
});
export const showUploadFilesModal = createAsyncThunk(
    "showUploadFilesModal",
    ({ owner, crumbs }, thunkAPI) => {
        thunkAPI.dispatch(showModal(modalTypes.uploadFiles, { owner, crumbs }));
    }
);