import { getRoute } from "../../utils/get-route";

const organizationRoutes = {
    fetchOrganizations: () => getRoute(`organizations/`)
};

export default organizationRoutes;