import { createSlice } from "@reduxjs/toolkit";
import {
    onPendingEvents,
    onFulfilledEvents,
    onRejectedEvents,
    onPendingEvent,
    onFulfilledEvent,
    onRejectedEvent,
    onPendingEventBreadcrumbs,
    onFulfilledEventBreadcrumbs,
    onRejectedEventBreadcrumbs } from "./defaults";
import { fetchEvents,
    fetchEvent,
    selectEvent,
    setGridView,
    fetchEventBreadcrumbs,
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
    isFetchingEventBreadcrumbs: false,
    hasError: false,
};

const eventSlice = createSlice({
    name: "eventSlice",
    initialState: initialState,
    extraReducers: {
        [fetchEvents.pending]: (state) => {
            onPendingEvents(state);
        },
        [fetchEvents.fulfilled]: (state, { payload: { events, hasError } }) => {
            onFulfilledEvents(state, hasError);

            if (!hasError) {
                state.events = events;
            }
        },
        [fetchEvents.rejected]: (state) => {
            onRejectedEvents(state);
            state.events = [];
            state.selectedEvent = null;
        },

        [fetchEvent.pending]: (state) => {
            onPendingEvent(state);
        },
        [fetchEvent.fulfilled]: (state, { payload: { event, hasError } }) => {
            onFulfilledEvent(state, hasError);

            if (!hasError) {
                state.eventInfo = event;
            }
        },
        [fetchEvent.rejected]: (state) => {
            onRejectedEvent(state);
            state.eventInfo = null;
        },

        [selectEvent]: (state, { payload: { event } }) => {
            state.selectedEvent = event;
        },
        [setGridView]: (state, { payload: { isGridView } }) => {
            state.isGridView = isGridView;
        },

        [fetchEventBreadcrumbs.pending]: (state) => {
            onPendingEventBreadcrumbs(state);
        },
        [fetchEventBreadcrumbs.fulfilled]: (state, { payload: { crumbs, hasError } }) => {
            onFulfilledEventBreadcrumbs(state, hasError);
            state.crumbs = crumbs;
        },
        [fetchEventBreadcrumbs.rejected]: (state) => {
            onRejectedEventBreadcrumbs(state);
            state.crumbs = [];
        },

        [updateEvent.pending]: (state) => {
            onPendingEvent(state);
        },
        [updateEvent.fulfilled]: (state, { payload: { updatedEvent, hasError } }) => {
            onFulfilledEvent(state, hasErrorEvent);

            if (!hasError) {
                state.eventInfo = updatedEvent;
            }
        },
        [updateEvent.rejected]: (state) => {
            onRejectedEvent(state);
            state.eventInfo = null;
        },

        [patchEvent.pending]: (state) => {
            onPendingEvent(state);
        },
        [patchEvent.fulfilled]: (state, { payload: { updatedEvent, hasError } }) => {
            onFulfilledEvent(state, hasError);

            if (!hasError) {
                state.eventInfo = updatedEvent;
            }
        },
        [patchEvent.rejected]: (state) => {
            onRejectedEvent(state);
            state.eventInfo = null;
        },
    }
});

export default eventSlice.reducer;