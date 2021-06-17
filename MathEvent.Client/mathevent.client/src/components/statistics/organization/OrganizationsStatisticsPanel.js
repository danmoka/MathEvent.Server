import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchStatistics } from '../../../store/actions/organization';
import BarChart from '../../_common/Chart/BarChart';
import Loader from '../../_common/Loader';
import PieChart from '../../_common/Chart/PieChart';
import './OrganizationsStatistics.scss';

const OrganizationsStatisticsPanel = () => {
  const dispatch = useDispatch();
  const { statistics, isFetchingOrganizationStatistics } = useSelector(
    (state) => state.organization
  );

  useEffect(() => {
    dispatch(fetchStatistics());
  }, [dispatch]);

  return isFetchingOrganizationStatistics ? (
    <Loader className="organizations-statistics-panel__loader" size="medium" />
  ) : (
    <div className="organizations-statistics-panel">
      {statistics.map((chart, index) => {
        switch (chart.type) {
          case 'pie':
            return (
              <div key={index} className="organizations-statistics-panel__item">
                <PieChart
                  data={chart.data}
                  title={chart.title}
                  valueField={chart.valueField}
                  argumentField={chart.argumentField}
                />
              </div>
            );
          case 'bar':
            return (
              <div key={index} className="organizations-statistics-panel__item">
                <BarChart
                  data={chart.data}
                  title={chart.title}
                  valueField={chart.valueField}
                  argumentField={chart.argumentField}
                />
              </div>
            );
          default:
            return (
              <div key={index} className="organizations-statistics-panel__item">
                <PieChart
                  data={chart.data}
                  title={chart.title}
                  valueField={chart.valueField}
                  argumentField={chart.argumentField}
                />
              </div>
            );
        }
      })}
    </div>
  );
};

export default OrganizationsStatisticsPanel;
