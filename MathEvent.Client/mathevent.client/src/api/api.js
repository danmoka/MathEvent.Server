import eventRoutes from "./routes/event-routes";
import organizationRoutes from "./routes/organization-routes";
import fileRoutes from "./routes/file-routes";

const api = {
    events: eventRoutes,
    organizations: organizationRoutes,
    files: fileRoutes,
};

export default api;