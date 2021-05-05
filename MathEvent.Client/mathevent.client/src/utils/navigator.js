import routes from "./routes";
import { history } from "../components/_app/AppContainer";

export const navigateToHome = () => history.push(routes.home);

export const navigateToLogin = () => history.push(routes.account.login);
export const navigateToRegister = () => history.push(routes.account.register);

export const navigateToEvents = () => history.push(routes.events.main);
export const navigateToEventEdit = (eventId) => history.push(`${routes.events.edit(eventId)}`);

export const navigateToEventsStatistics = () => history.push(routes.statistics.event);