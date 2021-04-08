import { createSlice } from "@reduxjs/toolkit";
import { onPendingDefault, onFulfilledDefault, onRejectedDefault } from "./defaults";
import { fetchEvents, fetchEvent, selectEvent, setGridView, fetchBreadcrumbs } from "../actions/event";

const initialState = {
    events: [],
    selectedEvent: null,
    crumbs: [],
    isGridView: true,
    isFetchingEvents: false,
    isFetchingEvent: false,
    isFetchingBreadcrumbs: false,
    hasError: false,
    eventInfo: null,
};

const eventSlice = createSlice({
    name: "eventSlice",
    initialState: initialState,
    extraReducers: {
        [fetchEvents.pending]: (state) => {
            onPendingDefault(state, state.isFetchingEvents);
        },
        [fetchEvents.fulfilled]: (state, { payload: { events, hasError } }) => {
            onFulfilledDefault(state, hasError, state.isFetchingEvents);
            state.events = events;
        },
        [fetchEvents.rejected]: (state) => {
            onRejectedDefault(state, state.isFetchingEvents);
            state.events = [];
            state.selectedEvent = null;
        },
        [fetchEvent.pending]: (state) => {
            onPendingDefault(state, state.isFetchingEvent);
        },
        [fetchEvent.fulfilled]: (state, { payload: { event, hasError } }) => {
            onFulfilledDefault(state, hasError, state.isFetchingEvent);
            state.eventInfo = event;
        },
        [fetchEvent.rejected]: (state) => {
            onRejectedDefault(state, state.isFetchingEvent);
            state.eventInfo = null;
        },
        [selectEvent]: (state, { payload: { event } }) => {
            state.selectedEvent = event;
        },
        [setGridView]: (state, { payload: { isGridView } }) => {
            state.isGridView = isGridView;
        },
        [fetchBreadcrumbs.pending]: (state) => {
            onPendingDefault(state, state.isFetchingBreadcrumbs);
        },
        [fetchBreadcrumbs.fulfilled]: (state, { payload: { crumbs, hasError } }) => {
            onFulfilledDefault(state, hasError, state.isFetchingBreadcrumbs);
            state.crumbs = crumbs;
        },
        [fetchBreadcrumbs.rejected]: (state) => {
            onRejectedDefault(state, state.isFetchingBreadcrumbs);
            state.crumbs = [];
        }
    }
});

export default eventSlice.reducer;