import { createSlice } from "@reduxjs/toolkit";
import { onPendingDefault, onFulfilledDefault, onRejectedDefault } from "./defaults";
import { fetchEvents, fetchEvent, selectEvent, setGridView } from "../actions/event";

const initialState = {
    events: [],
    selectedEvent: null,
    eventInfo: null,
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
        [fetchEvent.pending]: (state) => {
            onPendingDefault(state);
        },
        [fetchEvent.fulfilled]: (state, { payload: { event, hasError } }) => {
            onFulfilledDefault(state, hasError);
            state.eventInfo = event;
        },
        [fetchEvent.rejected]: (state) => {
            onRejectedDefault(state);
            state.eventInfo = null;
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