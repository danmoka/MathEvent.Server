import { getRoute } from "../../utils/get-route";

const eventRoutes = {
    fetchEvents: (event=null) => event
        ? getRoute(`events/?parent=${event.id}`)
        : getRoute(`events/?parent=${null}`),
    fetchEvent: (eventId) => getRoute(`events/${eventId}`)
};

export default eventRoutes;