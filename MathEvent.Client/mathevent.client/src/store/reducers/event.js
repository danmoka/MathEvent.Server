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
    onRejectedEventBreadcrumbs,
    onPendingEventsStatistics,
    onFulfilledEventsStatistics,
    onRejectedEventsStatistics,
    onPendingEventStatistics,
    onFulfilledEventStatistics,
    onRejectedEventStatistics
} from "./defaults";
import { fetchEvents,
    fetchEvent,
    createEvent,
    selectEvent,
    setGridView,
    fetchEventBreadcrumbs,
    fetchEventStatistics,
    fetchStatistics,
    updateEvent,
    patchEvent,
    deleteEvent,
    uploadEventAvatar,
} from "../actions/event";

const initialState = {
    events: [],
    eventInfo: null,
    selectedEvent: null,
    crumbs: [],
    eventStatistics: [],
    statistics: [],
    isGridView: true,
    isFetchingEvents: false,
    isFetchingEvent: false,
    isFetchingEventBreadcrumbs: false,
    isFetchingEventsStatistics: false,
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
        [createEvent.pending]: (state) => {
            onPendingEvent(state);
        },
        [createEvent.fulfilled]: (state, { payload: { createdEvent, hasError } }) => {
            onFulfilledEvent(state, hasError);

            if (!hasError) {
                state.eventInfo = createdEvent;
            }
        },
        [createEvent.rejected]: (state) => {
            onRejectedEvent(state);
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

        [fetchStatistics.pending]: (state) => {
            onPendingEventsStatistics(state);
        },
        [fetchStatistics.fulfilled]: (state, { payload: { statistics, hasError } }) => {
            onFulfilledEventsStatistics(state, hasError);

            if (!hasError) {
                state.statistics = statistics;
            }
        },
        [fetchStatistics.rejected]: (state) => {
            onRejectedEventsStatistics(state);
            state.statistics = [];
        },

        [fetchEventStatistics.pending]: (state) => {
            onPendingEventStatistics(state);
        },
        [fetchEventStatistics.fulfilled]: (state, { payload: { statistics, hasError } }) => {
            onFulfilledEventStatistics(state, hasError);

            if (!hasError) {
                state.eventStatistics = statistics;
            }
        },
        [fetchEventStatistics.rejected]: (state) => {
            onRejectedEventStatistics(state);
            state.eventStatistics = [];
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

        [deleteEvent.pending]: (state) => {
            onPendingEvent(state);
        },
        [deleteEvent.fulfilled]: (state, { payload: { eventId, hasError } }) => {
            onFulfilledEvent(state, hasError);
        
            if (!hasError) {
                state.events = state.events.filter((event) => event.id !== eventId);

                if (state.selectedEvent && state.selectedEvent.id === eventId) state.selectedEvent = state.events[0];
                if (state.eventInfo && state.eventInfo.id === eventId) state.eventInfo = null;
            }
        },
        [deleteEvent.rejected]: (state) => {
            onRejectedEvent(state);
        },

        [uploadEventAvatar.pending]: (state) => {
            onPendingEvent(state);
        },
        [uploadEventAvatar.fulfilled]: (state, { payload: { updatedEvent, hasError } }) => {
            onFulfilledEvent(state, hasError);

            if (!hasError) {
                state.eventInfo = updatedEvent;
            }
        },
        [uploadEventAvatar.rejected]: (state) => {
            onRejectedEvent(state);
        },
    }
});

export default eventSlice.reducer;