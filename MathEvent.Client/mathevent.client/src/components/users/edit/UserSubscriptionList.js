import React, { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import moment from 'moment';
import 'moment/locale/ru';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import {
  fetchEvent,
  fetchEvents,
  fetchEventBreadcrumbs,
  selectEvent,
} from '../../../store/actions/event';
import { patchUser } from '../../../store/actions/user';
import { navigateToEvents } from '../../../utils/navigator';
import { iconTypes } from '../../_common/Icon';
import List from '../../_common/List';
import './UserEdit.scss';

const prepareEvents = (events, onUnsubscribe, onClick) =>
  events.map((event, index) => ({
    id: event.id,
    primaryText: event.name,
    secondaryText: moment(event.startDate).format('LL'),
    index: index + 1,
    onClick: () => onClick(event),
    actions: [
      {
        id: 'unsubscribe',
        label: 'Отписаться',
        icon: iconTypes.personAddDisabled,
        onClick: () => onUnsubscribe(event),
      },
    ],
  }));

const UserSubscriptionList = () => {
  const dispatch = useDispatch();
  const { userInfo } = useSelector((state) => state.user);
  const [events, setEvents] = useState([]);
  const [userId, setUserId] = useState(null);

  useEffect(() => {
    if (userInfo) {
      setUserId(userInfo.id);
      setEvents(userInfo.events);
    }
  }, [userInfo]);

  const handleEventClick = useCallback((event) => {
    dispatch(selectEvent(event));
    dispatch(fetchEvent(event.id));

    if (event.hierarchy) {
      dispatch(fetchEventBreadcrumbs(event.id));
      dispatch(fetchEvents(event.id));
    } else {
      const parentId = event ? event.parentId : null;
      dispatch(fetchEvents(parentId));
      dispatch(fetchEventBreadcrumbs(parentId));
    }
    navigateToEvents();
  }, []);

  const handlePatchUser = useCallback(
    (data) => {
      dispatch(
        patchUser({
          userId,
          data,
        })
      );
    },
    [dispatch, userId]
  );

  const handleUnsibscribe = useCallback(
    (event) => {
      const eventIds = events
        .map((ev) => ev.id)
        .filter((id) => id !== event.id);

      handlePatchUser([
        {
          value: eventIds,
          path: '/Events',
          op: 'replace',
        },
      ]);
    },
    [handlePatchUser, events]
  );

  const preparedEvents = prepareEvents(
    events,
    handleUnsibscribe,
    handleEventClick
  );

  return (
    <div className="user-subscriptions-list">
      <Paper className="user-subscriptions-list__header">
        <Typography variant="h5" gutterBottom>
          Подписки
        </Typography>
      </Paper>
      <Paper className="user-subscriptions-list__items">
        <List className="user-subscriptions-list__ul" items={preparedEvents} />
      </Paper>
    </div>
  );
};

export default UserSubscriptionList;
