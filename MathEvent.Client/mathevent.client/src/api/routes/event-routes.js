import { getRoute } from "../../utils/get-route";

const eventRoutes = {
    fetchEvents: (eventId=null) => getRoute(`events/?parent=${eventId}`),
    fetchEvent: (eventId) => getRoute(`events/${eventId}`),
    createEvent: () => getRoute(`events/`),
    updateEvent: (eventId) => getRoute(`events/${eventId}/`),
    patchEvent: (eventId) => getRoute(`events/${eventId}/`),
    deleteEvent: (eventId) => getRoute(`events/${eventId}/`),
    fetchEventBreadcrumbs: (eventId) => getRoute(`events/breadcrumbs/${eventId}`),
    fetchStatistics: (eventSubsStatisticsTop) => getRoute(`events/statistics/?eventSubsStatisticsTop=${eventSubsStatisticsTop}`),
    fetchEventStatistics: (eventId) => getRoute(`events/statistics/${eventId}`)
};

export default eventRoutes;