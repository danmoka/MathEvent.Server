import { getRoute } from "../../utils/get-route";

const eventRoutes = {
    fetchEvents: () => getRoute("events"),
    fetchEvent: (eventId) => getRoute(`events/${eventId}`) 
};

export default eventRoutes;