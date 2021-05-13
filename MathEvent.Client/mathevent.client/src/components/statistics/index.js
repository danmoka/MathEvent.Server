import React from 'react';
import { useSelector } from 'react-redux';
import { Route } from 'react-router-dom';
import { useTitle } from '../../hooks';
import {
  navigateToEventsStatistics,
  navigateToOrganizationsStatistics,
  navigateToUsersStatistics,
} from '../../utils/navigator';
import routes from '../../utils/routes';
import TabPanel from '../_common/TabPanel';
import './Statistics.scss';
import EventsStatisticsPanel from './event/EventsStatisticsPanel';
import OrganizationsStatisticsPanel from './organization/OrganizationsStatisticsPanel';
import UsersStatisticsPanel from './user/UsersStatisticsPanel';

const tabRoutes = [
  routes.statistics.events,
  routes.statistics.organizations,
  routes.statistics.users,
];

const tabs = [
  { label: 'События', onClick: () => navigateToEventsStatistics() },
  { label: 'Организации', onClick: () => navigateToOrganizationsStatistics() },
  { label: 'Пользователи', onClick: () => navigateToUsersStatistics() },
];

const Statistics = () => {
  const currentRoute = useSelector((state) => state.router.location.pathname);

  useTitle('Статистика');

  return (
    <div className="statistics">
      <div className="statistics__tabs">
        <TabPanel tabs={tabs} value={tabRoutes.indexOf(currentRoute)} />
      </div>
      <div className="statistics__content">
        <Route
          path={routes.statistics.events}
          component={EventsStatisticsPanel}
        />
        <Route
          path={routes.statistics.organizations}
          component={OrganizationsStatisticsPanel}
        />
        <Route
          path={routes.statistics.users}
          component={UsersStatisticsPanel}
        />
      </div>
    </div>
  );
};

export default Statistics;
