import React from 'react';
import { Route } from 'react-router-dom';
import UserEdit from '../edit/UserEdit';
import routes from '../../../utils/routes';

const Users = () => (
  <>
    <Route
      path={`${routes.users.edit(':id')}`}
      exact
      render={(props) => <UserEdit {...props} />}
    />
  </>
);

export default Users;
