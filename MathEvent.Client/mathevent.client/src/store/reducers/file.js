import { createSlice } from "@reduxjs/toolkit";
import {
    onPendingFiles,
    onFulfilledFiles,
    onRejectedFiles,
    onPendingFile,
    onFulfilledFile,
    onRejectedFile,
    onPendingFileBreadcrumbs,
    onFulfilledFileBreadcrumbs,
    onRejectedFileBreadcrumbs } from "./defaults";
import {
    fetchFiles,
    fetchFile,
    fetchFileBreadcrumbs,
    deleteFile,
    createFile,
    uploadFiles} from "../actions/file";

const initialState = {
    files: [],
    fileInfo: null,
    crumbs: [],
    isFetchingFiles: false,
    isFetchingFile: false,
    isFetchingFileBreadcrumbs: false,
    hasError: false,
};

const fileSlice = createSlice({
    name: "fileSlice",
    initialState: initialState,
    extraReducers: {
        [fetchFiles.pending]: (state) => {
            onPendingFiles(state);
        },
        [fetchFiles.fulfilled]: (state, { payload: { files, hasError } }) => {
            onFulfilledFiles(state, hasError);

            if (!hasError) {
                state.files = files;
            }
        },
        [fetchFiles.rejected]: (state) => {
            onRejectedFiles(state);
            state.files = [];
        },

        [fetchFile.pending]: (state) => {
            onPendingFile(state);
        },
        [fetchFile.fulfilled]: (state, { payload: { file, hasError } }) => {
            onFulfilledFile(state, hasError);

            if (!hasError) {
                state.fileInfo = file;
            }
        },
        [fetchFile.rejected]: (state) => {
            onRejectedFile(state);
            state.fileInfo = null;
        },

        [fetchFileBreadcrumbs.pending]: (state) => {
            onPendingFileBreadcrumbs(state);
        },
        [fetchFileBreadcrumbs.fulfilled]: (state, { payload: { crumbs, hasError } }) => {
            onFulfilledFileBreadcrumbs(state, hasError);
            state.crumbs = crumbs;
        },
        [fetchFileBreadcrumbs.rejected]: (state) => {
            onRejectedFileBreadcrumbs(state);
            state.crumbs = [];
        },

        [createFile.pending]: (state) => {
            onPendingFile(state);
        },
        [createFile.fulfilled]: (state, { payload: { createdFile, hasError } }) => {
            onFulfilledFile(state, hasError);

            if (!hasError) {
                state.fileInfo = createdFile;
            }
        },
        [createFile.rejected]: (state) => {
            onRejectedFile(state);
        },

        [deleteFile.pending]: (state) => {
            onPendingFile(state);
        },
        [deleteFile.fulfilled]: (state, { payload: { fileId, hasError } }) => {
            onFulfilledFile(state, hasError);

            if (!hasError) {
                state.files = state.files.filter((file) => file.id !== fileId);

                if (state.fileInfo && state.fileInfo.id === fileId) state.fileInfo = null;
            }
        },
        [deleteFile.rejected]: (state) => {
            onRejectedFile(state);
        },

        [uploadFiles.pending]: (state) => {
            onPendingFile(state);
        },
        [uploadFiles.fulfilled]: (state, { payload: { hasError } }) => {
            onFulfilledFile(state, hasError);

            if (!hasError) {
                state.fileInfo = null;
            }
        },
        [uploadFiles.rejected]: (state) => {
            onRejectedFile(state);
        },
    }
});

export default fileSlice.reducer;