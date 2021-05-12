import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useParams } from 'react-router';
import { fetchUser } from '../../../store/actions/user';
import { useCurrentUser } from '../../../hooks';
import Loader from '../../_common/Loader';
import UserEditInfo from './UserEditInfo';
import UserSubscriptionList from './UserSubscriptionList';
import './UserEdit.scss';

const UserEdit = () => {
  const dispatch = useDispatch();
  const { id } = useParams();
  const { userInfo, isAuthenticated } = useCurrentUser();
  const { isFetchingUser } = useSelector((state) => state.user);

  useEffect(() => {
    if (isAuthenticated && id === userInfo.sub) {
      // todo: изменить проверку на: аутентифицирован и (совпадают id или в роли админа)
      dispatch(fetchUser(id));
    }
  }, [dispatch, id, userInfo, isAuthenticated]);

  return isFetchingUser ? (
    <Loader className="user-edit__loader" size="medium" />
  ) : (
    <div className="user-edit">
      <UserSubscriptionList />
      <UserEditInfo />
    </div>
  );
};

export default UserEdit;
