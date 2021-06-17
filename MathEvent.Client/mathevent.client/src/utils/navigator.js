import routes from './routes';
import { history } from '../components/_app/AppContainer';

export const navigateToHome = () => history.push(routes.home);

export const navigateToLogin = () => history.push(routes.account.login);
export const navigateToRegister = () => history.push(routes.account.register);
export const navigateToUserEdit = (userId) =>
  history.push(`${routes.users.edit(userId)}`);

export const navigateToEvents = () => history.push(routes.events.main);
export const navigateToEventEdit = (eventId) =>
  history.push(`${routes.events.edit(eventId)}`);

export const navigateToEventsStatistics = () =>
  history.push(routes.statistics.events);
export const navigateToOrganizationsStatistics = () =>
  history.push(routes.statistics.organizations);
export const navigateToUsersStatistics = () =>
  history.push(routes.statistics.users);
