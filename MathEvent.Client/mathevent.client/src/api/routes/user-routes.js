import { getRoute } from "../../utils/get-route";

const userRoutes = {
    fetchUsers: () => getRoute(`users/`),
    registerUrl: () => getRoute(`users/`),
    fetchStatistics: (activeUsersTop) => getRoute(`users/statistics/?activeUsersTop=${activeUsersTop}`)
};

export default userRoutes;