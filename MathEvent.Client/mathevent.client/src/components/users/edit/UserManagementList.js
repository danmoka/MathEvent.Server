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

const prepareEvents = (events, onStopManaging, onClick) =>
  events.map((event, index) => ({
    id: event.id,
    primaryText: event.name,
    secondaryText: moment(event.startDate).format('LL'),
    index: index + 1,
    onClick: () => onClick(event),
    actions: [
      {
        id: 'stopManaging',
        label: 'Прекратить управление',
        icon: iconTypes.close,
        onClick: () => onStopManaging(event),
      },
    ],
  }));

const UserManagementList = () => {
  const dispatch = useDispatch();
  const { userInfo } = useSelector((state) => state.user);
  const [managedEvents, setManagedEvents] = useState([]);
  const [userId, setUserId] = useState(null);

  useEffect(() => {
    if (userInfo) {
      setUserId(userInfo.id);
      setManagedEvents(userInfo.managedEvents);
    }
  }, [userInfo]);

  const handleEventClick = useCallback((event) => {
    dispatch(selectEvent(event));
    dispatch(fetchEvent(event.id));

    if (event.hierarchy) {
      dispatch(fetchEventBreadcrumbs(event.id));
      dispatch(fetchEvents(event.id));
    } else {
      const { parentId } = event;
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

  const handleStopManaging = useCallback(
    (event) => {
      const eventIds = managedEvents
        .map((ev) => ev.id)
        .filter((id) => id !== event.id);

      handlePatchUser([
        {
          value: eventIds,
          path: '/ManagedEvents',
          op: 'replace',
        },
      ]);
    },
    [handlePatchUser, managedEvents]
  );

  const preparedMananedEvents = prepareEvents(
    managedEvents,
    handleStopManaging,
    handleEventClick
  );

  return (
    <div className="user-events-list">
      <Paper className="user-events-list__header">
        <Typography variant="h5" gutterBottom>
          Управление
        </Typography>
      </Paper>
      <Paper className="user-events-list__items">
        <List className="user-events-list__ul" items={preparedMananedEvents} />
      </Paper>
    </div>
  );
};

export default UserManagementList;
