import { getRoute } from "../../utils/get-route";

const userRoutes = {
    fetchUsers: () => getRoute(`users/`),
};

export default userRoutes;