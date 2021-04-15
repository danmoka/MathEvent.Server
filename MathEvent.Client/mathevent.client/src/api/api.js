import eventRoutes from "./routes/event-routes";
import organizationRoutes from "./routes/organization-routes";

const api = {
    events: eventRoutes,
    organizations: organizationRoutes
};

export default api;