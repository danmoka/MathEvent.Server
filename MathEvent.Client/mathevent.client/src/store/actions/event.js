import { createAction, createAsyncThunk } from "@reduxjs/toolkit";
import eventService from "../../api/services/event-service";
import statusCode from "../../utils/status-code-reader";

export const fetchEvents = createAsyncThunk("fetchEvents", async () => {
    const response = await eventService.fetchEvents();

    if (statusCode(response).ok) {
        const events = await response.json();

        return { events }
    }

    return { events: [] }
})

export const selectEvent = createAction("selectEvent", (event) => ({ payload: { event } }));