import React from 'react';
import { Route, Switch } from 'react-router-dom';
import Events from '../events';
import Home from '../home';
import Login from '../account/Login';
import Register from '../account/Register';
import ModalRoot from '../_common/Modal/ModalRoot';
import Statistics from '../statistics';
import Users from '../users';
import routes from '../../utils/routes';

const AppContent = () => (
  <div>
    <Switch>
      <Route path="/" exact component={Home} />
      <Route path={routes.home} component={Home} />
      <Route path={routes.events.main} component={Events} />
      <Route path={routes.account.login} component={Login} />
      <Route path={routes.account.register} component={Register} />
      <Route path={routes.statistics.main} component={Statistics} />
      <Route path={routes.users.main} component={Users} />
    </Switch>
    <ModalRoot />
  </div>
);

export default AppContent;
