import { createSlice } from "@reduxjs/toolkit";
import { onPendingDefault, onFulfilledDefault, onRejectedDefault } from "./defaults";
import { fetchEvents,
    fetchEvent,
    selectEvent,
    setGridView,
    fetchBreadcrumbs,
    updateEvent,
    patchEvent} from "../actions/event";

const initialState = {
    events: [],
    eventInfo: null,
    selectedEvent: null,
    crumbs: [],
    isGridView: true,
    isFetchingEvents: false,
    isFetchingEvent: false,
    isFetchingBreadcrumbs: false,
    hasError: false,
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

            if (!hasError) {
                state.events = events;
            }
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

            if (!hasError) {
                state.eventInfo = event;
            }
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
        },

        [updateEvent.pending]: (state) => {
            onPendingDefault(state, state.isFetchingEvent);
        },
        [updateEvent.fulfilled]: (state, { payload: { updatedEvent, hasError } }) => {
            onFulfilledDefault(state, hasError, state.isFetchingEvent);

            if (!hasError) {
                state.eventInfo = updatedEvent;
            }
        },
        [updateEvent.rejected]: (state) => {
            onRejectedDefault(state, state.isFetchingEvent);
            state.eventInfo = null;
        },

        [patchEvent.pending]: (state) => {
            onPendingDefault(state, state.isFetchingEvent);
        },
        [patchEvent.fulfilled]: (state, { payload: { updatedEvent, hasError } }) => {
            onFulfilledDefault(state, hasError, state.isFetchingEvent);

            if (!hasError) {
                state.eventInfo = updatedEvent;
            }
        },
        [patchEvent.rejected]: (state) => {
            onRejectedDefault(state, state.isFetchingEvent);
            state.eventInfo = null;
        },
    }
});

export default eventSlice.reducer;