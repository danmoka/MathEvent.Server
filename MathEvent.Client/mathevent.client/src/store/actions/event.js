import { createAction, createAsyncThunk } from "@reduxjs/toolkit";
import eventService from "../../api/services/event-service";
import statusCode from "../../utils/status-code-reader";

export const fetchEvents = createAsyncThunk("fetchEvents", async (eventId) => {
    const response = await eventService.fetchEvents(eventId);

    if (statusCode(response).ok) {
        const events = await response.json();

        return { events };
    }

    return { events: [] };
});

export const fetchEvent = createAsyncThunk("fetchEvent", async (eventId) => {
    const response = await eventService.fetchEvent(eventId);

    if (statusCode(response).ok) {
        const event = await response.json();

        return { event };
    }

    return { event: null };
});

export const fetchBreadcrumbs = createAsyncThunk("fetchBreadcrumbs", async (eventId) => {
    if (!eventId) {
        return { crumbs: [] };
    }

    const response = await eventService.fetchBreadcrumbs(eventId);

    if (statusCode(response).ok) {
        const crumbs = await response.json();

        return { crumbs };
    }

    return { crumbs: [] };
});

export const updateEvent = createAsyncThunk("updateEvent", async ({ eventId, event }) => {
    const response = await eventService.updateEvent(eventId, event);

    if (statusCode(response).ok) {
      const updatedEvent = await response.json();

      return { updatedEvent, hasError: false };
    }

    return { hasError: true };
});

export const patchEvent = createAsyncThunk("patchEvent", async ({ eventId, data }) => {
    const response = await eventService.patchEvent(eventId, data);

    if (statusCode(response).ok) {
      const updatedEvent = await response.json();

      return { updatedEvent, hasError: false };
    }

    return { hasError: true };
});

export const selectEvent = createAction("selectEvent", (event) => ({ payload: { event } }));
export const setGridView = createAction("setGridView", (isGridView) => ({ payload: { isGridView } }));