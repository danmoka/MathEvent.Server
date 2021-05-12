/* eslint-disable react/no-array-index-key */
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchUserStatistics } from '../../../store/actions/user';
import { ShowModal, modalSizes } from '../../_common/Modal';
import BarChart from '../../_common/Chart/BarChart';
import Loader from '../../_common/Loader';
import PieChart from '../../_common/Chart/PieChart';
import './UsersStatistics.scss';

const UserStatisticsModal = () => {
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.modal.modalProps);
  const { userStatistics, isFetchingUserStatistics } = useSelector(
    (state) => state.user
  );

  useEffect(() => {
    dispatch(fetchUserStatistics(user.sub));
  }, [dispatch, user]);

  return (
    <ShowModal
      title={`Статистика пользователя ${user.name}`}
      size={modalSizes.small}
    >
      {isFetchingUserStatistics ? (
        <Loader className="user-statistics-panel__loader" size="medium" />
      ) : (
        <div className="user-statistics-panel">
          {userStatistics.map((chart, index) => {
            switch (chart.type) {
              case 'pie':
                return (
                  <div key={index} className="user-statistics-panel__item">
                    <PieChart
                      data={chart.data}
                      title={chart.title}
                      valueField={chart.valueField}
                      argumentField={chart.argumentField}
                      elevation={0}
                    />
                  </div>
                );
              case 'bar':
                return (
                  <div key={index} className="user-statistics-panel__item">
                    <BarChart
                      data={chart.data}
                      title={chart.title}
                      valueField={chart.valueField}
                      argumentField={chart.argumentField}
                      elevation={0}
                    />
                  </div>
                );
              default:
                return (
                  <div key={index} className="user-statistics-panel__item">
                    <PieChart
                      data={chart.data}
                      title={chart.title}
                      valueField={chart.valueField}
                      argumentField={chart.argumentField}
                      elevation={0}
                    />
                  </div>
                );
            }
          })}
        </div>
      )}
    </ShowModal>
  );
};

export default UserStatisticsModal;
