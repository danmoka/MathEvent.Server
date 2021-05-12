import React, { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import moment from 'moment';
import 'moment/locale/ru';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import {
  selectEvent,
  fetchEvent,
  fetchEvents,
  fetchEventBreadcrumbs,
} from '../../../store/actions/event';
import { fetchOrganizations } from '../../../store/actions/organization';
import { patchUser } from '../../../store/actions/user';
import { navigateToEvents } from '../../../utils/navigator';
import { iconTypes } from '../../_common/Icon';
import Dropdown from '../../_common/Dropdown';
import List from '../../_common/List';
import TextField from '../../_common/TextField';
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

const prepareOrganizations = (organizations) => [
  { value: '', name: 'Без организации' },
  ...organizations.map((organization) => ({
    value: organization.id,
    name: organization.name,
  })),
];

const UserEditInfo = () => {
  const dispatch = useDispatch();
  const { userInfo } = useSelector((state) => state.user);
  const { organizations } = useSelector((state) => state.organization);
  const preparedOrganizations = prepareOrganizations(organizations);

  const [userId, setUserId] = useState(null);
  const [name, setName] = useState('');
  const [surname, setSurname] = useState('');
  const [patronymic, setPatronymic] = useState('');
  const [email, setEmail] = useState('');
  const [userName, setUserName] = useState('');
  const [organization, setOrganization] = useState(
    preparedOrganizations[0].value
  );
  const [managedEvents, setManagedEvents] = useState([]);

  useEffect(() => {
    dispatch(fetchOrganizations());
  }, []);

  useEffect(() => {
    if (userInfo) {
      setUserId(userInfo.id);
      setName(userInfo.name);
      setSurname(userInfo.surname);
      setPatronymic(userInfo.patronymic ? userInfo.patronymic : '');
      setEmail(userInfo.email);
      setUserName(userInfo.userName);
      setManagedEvents(userInfo.managedEvents);

      if (userInfo.organization) {
        setOrganization(userInfo.organization.id);
      }
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

  const handleNameValueChange = useCallback(
    (newName) => {
      setName(newName);
      handlePatchUser([
        {
          value: newName,
          path: '/Name',
          op: 'replace',
        },
      ]);
    },
    [handlePatchUser, userInfo]
  ); // userInfo мб не надо?

  const handleSurnameValueChange = useCallback(
    (newSurname) => {
      setSurname(newSurname);
      handlePatchUser([
        {
          value: newSurname,
          path: '/Surname',
          op: 'replace',
        },
      ]);
    },
    [handlePatchUser, userInfo]
  ); // userInfo мб не надо?

  const handlePatronymicValueChange = useCallback(
    (newPatronymic) => {
      setPatronymic(newPatronymic);
      handlePatchUser([
        {
          value: newPatronymic,
          path: '/Patronymic',
          op: 'replace',
        },
      ]);
    },
    [handlePatchUser, userInfo]
  );

  const handleEmailValueChange = useCallback(
    (newEmail) => {
      setEmail(newEmail);
      handlePatchUser([
        {
          value: newEmail,
          path: '/Email',
          op: 'replace',
        },
      ]);
    },
    [handlePatchUser, userInfo]
  );

  const handleUserNameValueChange = useCallback(
    (newUserName) => {
      setUserName(newUserName);
      handlePatchUser([
        {
          value: newUserName,
          path: '/UserName',
          op: 'replace',
        },
      ]);
    },
    [handlePatchUser, userInfo]
  );

  const handleOrganizationChange = useCallback(
    (newOrganization) => {
      setOrganization(newOrganization);
      handlePatchUser([
        {
          value: newOrganization || null,
          path: '/OrganizationId',
          op: 'replace',
        },
      ]);
    },
    [handlePatchUser, userInfo]
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
    <div className="user-edit-info">
      <section className="user-edit-info__info">
        <Paper className="user-edit-info__info--description">
          <TextField
            className="user-edit-form__control"
            label="Имя"
            value={name}
            onChange={handleNameValueChange}
          />
          <TextField
            className="user-edit-form__control"
            label="Фамилия"
            value={surname}
            onChange={handleSurnameValueChange}
          />
          <TextField
            className="user-edit-form__control"
            label="Отчество"
            value={patronymic}
            onChange={handlePatronymicValueChange}
          />
          <TextField
            className="user-edit-form__control"
            label="Email"
            value={email}
            onChange={handleEmailValueChange}
          />
          <TextField
            className="user-edit-form__control"
            label="Логин"
            value={userName}
            onChange={handleUserNameValueChange}
          />
          <Dropdown
            className="user-edit-form__control"
            label="Организация"
            value={organization}
            items={preparedOrganizations}
            onChange={handleOrganizationChange}
          />
        </Paper>
      </section>
      <section className="user-edit-info__management">
        <div className="user-managing-list">
          <Paper className="user-managing-list__header">
            <Typography variant="h5" gutterBottom>
              Управление
            </Typography>
          </Paper>
          <Paper className="user-managing-list__items">
            <List
              className="user-managing-list__ul"
              items={preparedMananedEvents}
            />
          </Paper>
        </div>
      </section>
    </div>
  );
};

export default UserEditInfo;
