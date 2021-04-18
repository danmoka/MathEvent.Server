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
    fetchFileBreadcrumbs} from "../actions/file";

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
    }
});

export default fileSlice.reducer;