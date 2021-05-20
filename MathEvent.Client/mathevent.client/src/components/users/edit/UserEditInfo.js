import React, { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useDebouncedCallback } from 'use-debounce';
import 'moment/locale/ru';
import Paper from '@material-ui/core/Paper';
import { fetchOrganizations } from '../../../store/actions/organization';
import { patchUser } from '../../../store/actions/user';
import Dropdown from '../../_common/Dropdown';
import TextField from '../../_common/TextField';
import UserFiles from './UserFiles';
import './UserEdit.scss';

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

      if (userInfo.organization) {
        setOrganization(userInfo.organization.id);
      }
    }
  }, [userInfo]);

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

  const handleNameValueChange = useDebouncedCallback((newName) => {
    setName(newName);
    handlePatchUser([
      {
        value: newName,
        path: '/Name',
        op: 'replace',
      },
    ]);
  }, 1000);

  const handleSurnameValueChange = useDebouncedCallback((newSurname) => {
    setSurname(newSurname);
    handlePatchUser([
      {
        value: newSurname,
        path: '/Surname',
        op: 'replace',
      },
    ]);
  }, 1000);

  const handlePatronymicValueChange = useDebouncedCallback((newPatronymic) => {
    setPatronymic(newPatronymic);
    handlePatchUser([
      {
        value: newPatronymic,
        path: '/Patronymic',
        op: 'replace',
      },
    ]);
  }, 1000);

  const handleEmailValueChange = useDebouncedCallback((newEmail) => {
    setEmail(newEmail);
    handlePatchUser([
      {
        value: newEmail,
        path: '/Email',
        op: 'replace',
      },
    ]);
  }, 1000);

  const handleUserNameValueChange = useDebouncedCallback((newUserName) => {
    setUserName(newUserName);
    handlePatchUser([
      {
        value: newUserName,
        path: '/UserName',
        op: 'replace',
      },
    ]);
  }, 1000);

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
      <section className="user-edit-info__files">
        <UserFiles />
      </section>
    </div>
  );
};

export default UserEditInfo;
