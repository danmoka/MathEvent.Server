import { getRoute } from "../../utils/get-route";

const organizationRoutes = {
    fetchOrganizations: () => getRoute(`organizations/`),
    fetchStatistics: () => getRoute(`organizations/statistics`)
};

export default organizationRoutes;