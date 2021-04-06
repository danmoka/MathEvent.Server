import { createAction, createAsyncThunk } from "@reduxjs/toolkit";
import eventService from "../../api/services/event-service";
import statusCode from "../../utils/status-code-reader";

export const fetchEvents = createAsyncThunk("fetchEvents", async (event) => {
    const response = await eventService.fetchEvents(event);

    if (statusCode(response).ok) {
        const events = await response.json();

        return { events };
    }

    return { events: [] };
});

export const selectEvent = createAction("selectEvent", (event) => ({ payload: { event } }));
export const setGridView = createAction("setGridView", (isGridView) => ({ payload: { isGridView } }));