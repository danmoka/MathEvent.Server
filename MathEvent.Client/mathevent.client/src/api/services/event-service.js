import api from "../api";
import baseService from "./base-service";

const eventService = {
    fetchEvents: async (event) => {
        const url = api.events.fetchEvents(event);

        return await baseService.get(url);
    }
};

export default eventService;