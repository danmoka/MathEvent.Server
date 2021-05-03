import routes from "./routes";
import { history } from "../components/_app/AppContainer";

export const navigateToHome = () => history.push(routes.home);

export const navigateToLogin = () => history.push(routes.login);
export const navigateToRegister = () => history.push(routes.register);

export const navigateToEvents = () => history.push(routes.events);
export const navigateToEventEdit = (eventId) => history.push(`${routes.events}/${eventId}/edit`);