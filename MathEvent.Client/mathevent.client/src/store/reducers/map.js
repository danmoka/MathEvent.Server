import { createSlice } from "@reduxjs/toolkit";
import {
    onPendingPosition,
    onFulfilledPosition,
    onRejectedPosition,
} from "./defaults";
import {
    fetchPosition
} from "../actions/map";

const initialState = {
    positionResults: [],
    isFetchingPosition: false
};

const mapSlice = createSlice({
    name: "mapSlice",
    initialState: initialState,
    extraReducers: {
        [fetchPosition.pending]: (state) => {
            onPendingPosition(state);
        },
        [fetchPosition.fulfilled]: (state, { payload: { results, hasError } }) => {
            onFulfilledPosition(state, hasError);

            if (!hasError) {
                state.positionResults = results;
            }
        },
        [fetchPosition.rejected]: (state) => {
            onRejectedPosition(state);
            state.positionResults = [];
        },
    }
});

export default mapSlice.reducer;