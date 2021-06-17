import { getAccountRoute } from "../../utils/get-route";

const accountRoutes = {
    tokenUrl: () => getAccountRoute("/connect/token"),
    revocationUrl: () => getAccountRoute("/connect/revocation"),
    infoUrl: () => getAccountRoute("/connect/userinfo"),
};

export default accountRoutes;