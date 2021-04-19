import accountRoutes from "./routes/account-routes";
import eventRoutes from "./routes/event-routes";
import organizationRoutes from "./routes/organization-routes";
import fileRoutes from "./routes/file-routes";

const api = {
    account: accountRoutes,
    events: eventRoutes,
    organizations: organizationRoutes,
    files: fileRoutes,
};

export default api;