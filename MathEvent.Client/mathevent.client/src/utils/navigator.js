import routes from "./routes";
import { history } from "../components/_app/AppContainer";

export const navigateToEvents = () => history.push(routes.events);
export const navigateToEventEdit = (eventId) => history.push(`${routes.events}/${eventId}/edit`);