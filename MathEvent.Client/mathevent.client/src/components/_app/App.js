import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchTokens, fetchUserInfo } from '../../store/actions/account';
import { fetchEvents } from '../../store/actions/event';
import AppContent from './AppContent';

const useInterval = (callback, delay) => {
  const savedCallback = useRef();

  useEffect(() => {
    savedCallback.current = callback;
  }, [callback]);

  // eslint-disable-next-line consistent-return
  useEffect(() => {
    const tick = () => {
      savedCallback.current();
    };

    if (delay !== null) {
      const id = setInterval(tick, delay);

      return () => clearInterval(id);
    }
  }, [delay]);
};

const tokenInterval = 1000 * 60 * 4;

const App = () => {
  const dispatch = useDispatch();
  const { hasToken } = useSelector((state) => state.account);

  useEffect(() => {
    dispatch(fetchTokens({ userName: null, password: null }));
  }, [dispatch]);

  useEffect(() => {
    dispatch(fetchUserInfo());
  }, [dispatch, hasToken]);

  useInterval(() => {
    dispatch(fetchTokens({ userName: null, password: null }));
  }, tokenInterval);

  useEffect(() => {
    dispatch(fetchEvents());
  }, [dispatch]);

  return <AppContent />;
};

export default App;
