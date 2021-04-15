import { getRoute } from "../../utils/get-route";

const eventRoutes = {
    fetchEvents: (eventId=null) => getRoute(`events/?parent=${eventId}`),
    fetchEvent: (eventId) => getRoute(`events/${eventId}`),
    updateEvent: (eventId) => getRoute(`events/${eventId}/`),
    patchEvent: (eventId) => getRoute(`events/${eventId}/`),
    fetchBreadcrumbs: (eventId) => getRoute(`events/breadcrumbs/${eventId}`)
};

export default eventRoutes;