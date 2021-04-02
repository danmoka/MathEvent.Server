import api from "../api";
import baseService from "./base-service";

const eventService = {
    fetchEvents: async () => {
        const url = api.events.fetchEvents();

        return await baseService.get(url);
    }
};

export default eventService;