import { getRoute } from "../../utils/get-route";

const userRoutes = {
    fetchUsers: () => getRoute(`users/`),
    registerUrl: () => getRoute(`users/`),
};

export default userRoutes;