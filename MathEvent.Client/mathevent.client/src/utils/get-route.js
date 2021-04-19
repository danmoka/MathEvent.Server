import urlJoin from "url-join";
import config from "../config";

export const getRoute = (route) => urlJoin(config.baseUrl, "api", route);
export const getAccountRoute = (route) => urlJoin(config.accountUrl, route);