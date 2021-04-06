import { createSlice } from "@reduxjs/toolkit";
import { onPendingDefault, onFulfilledDefault, onRejectedDefault } from "./defaults";
import { fetchEvents, selectEvent, setGridView } from "../actions/event";

const initialState = {
    events: [],
    selectedEvent: null,
    isGridView: true,
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
            state.selectedEvent = null;
        },
        [selectEvent]: (state, { payload: { event } }) => {
            state.selectedEvent = event;
        },
        [setGridView]: (state, { payload: { isGridView } }) => {
            state.isGridView = isGridView;
        }
    }
});

export default eventSlice.reducer;