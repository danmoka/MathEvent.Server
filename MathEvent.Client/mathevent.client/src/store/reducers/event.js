import { createSlice } from "@reduxjs/toolkit";
import { onPendingDefault, onFulfilledDefault, onRejectedDefault } from "./defaults";
import { fetchEvents } from "../actions/event";

const initialState = {
    events: [],
    isFetching: false,
    hasError: false
};

const eventSlice = createSlice({
    name: "eventSlice",
    initialState: initialState,
    extraReducers: {
        [fetchEvents.pending]: (state) => {
            onPendingDefault(state);
        },
        [fetchEvents.fulfilled]: (state, { payload: { events, hasError } }) => {
            onFulfilledDefault(state, hasError);
            state.events = events;
        },
        [fetchEvents.rejected]: (state) => {
            onRejectedDefault(state);
            state.events = [];
        }
    }
});

export default eventSlice.reducer;