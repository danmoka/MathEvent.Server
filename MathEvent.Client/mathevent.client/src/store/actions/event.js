import { createAction, createAsyncThunk } from "@reduxjs/toolkit";
import { showModal, hideModal } from "./modal";
import eventService from "../../api/services/event-service";
import statusCode from "../../utils/status-code-reader";
import modalTypes from "../../constants/modal-types";

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

export const fetchEventBreadcrumbs = createAsyncThunk("fetchEventBreadcrumbs", async (eventId) => {
    if (!eventId) {
        return { crumbs: [] };
    }

    const response = await eventService.fetchEventBreadcrumbs(eventId);

    if (statusCode(response).ok) {
        const crumbs = await response.json();

        return { crumbs };
    }

    return { crumbs: [] };
});

export const fetchStatistics = createAsyncThunk("fetchStatistics", async (eventSubsStatisticsTop) => {
    const response = await eventService.fetchStatistics(eventSubsStatisticsTop);

    if (statusCode(response).ok) {
        const statistics = await response.json();

        return { statistics };
    }

    return { statistics: [] };
});

export const createEvent = createAsyncThunk("createEvent", async ({ event }, thunkAPI) => {
    thunkAPI.dispatch(hideModal());
    const response = await eventService.createEvent(event);

    if (statusCode(response).created) {
        const createdEvent = await response.json();
        thunkAPI.dispatch(fetchEvents(createdEvent.parentId));

        return { createdEvent, hasError: false };
    }

    return { hasError: true };
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

export const deleteEvent = createAsyncThunk("deleteEvent", async ({ eventId }, thunkAPI) => {
    thunkAPI.dispatch(hideModal());
    const response = await eventService.deleteEvent(eventId);

    if (statusCode(response).noContent) {
        return { eventId, hasError: false };
    }
  
    return { hasError: true };
});

export const selectEvent = createAction("selectEvent", (event) => ({ payload: { event } }));
export const setGridView = createAction("setGridView", (isGridView) => ({ payload: { isGridView } }));

export const showCreateEventModal = createAsyncThunk("showCreateEventModal", async (params, thunkAPI) => {
    thunkAPI.dispatch(showModal(modalTypes.createEvent));
});
export const showDeleteEventModal = createAsyncThunk("showDeleteEventModal", async ({ event }, thunkAPI) => {
    thunkAPI.dispatch(showModal(modalTypes.deleteEvent, { event }));
});
export const showEditManagersEventModal = createAsyncThunk("showEditManagersEventModal", async ({ event, preparedNewManagers }, thunkAPI) => {
    thunkAPI.dispatch(showModal(modalTypes.editManagersEventModal, { event, preparedNewManagers }));
});