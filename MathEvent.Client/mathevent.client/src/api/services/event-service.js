import api from "../api";
import baseService from "./base-service";

const eventService = {
    fetchEvents: async (eventId) => {
        const url = api.events.fetchEvents(eventId);

        return await baseService.get(url);
    },
    fetchEvent: async (eventId) => {
        const url = api.events.fetchEvent(eventId);

        return await baseService.get(url);
    },
    fetchBreadcrumbs: async (eventId) => {
        const url = api.events.fetchBreadcrumbs(eventId);

        return await baseService.get(url);
    }
};

export default eventService;