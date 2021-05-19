import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchStatistics } from '../../../store/actions/event';
import BarChart from '../../_common/Chart/BarChart';
import Loader from '../../_common/Loader';
import PieChart from '../../_common/Chart/PieChart';
import './EventsStatistics.scss';

const EventsStatisticsPanel = () => {
  const dispatch = useDispatch();
  const { statistics, isFetchingEventsStatistics } = useSelector(
    (state) => state.event
  );
  const [eventSubsStatisticsTop, setEventSubsStatisticsTop] = useState(10);

  useEffect(() => {
    dispatch(fetchStatistics(eventSubsStatisticsTop));
  }, [dispatch, eventSubsStatisticsTop]);

  return isFetchingEventsStatistics ? (
    <Loader className="events-statistics-panel__loader" size="medium" />
  ) : (
    <div className="events-statistics-panel">
      {statistics.map((chart, index) => {
        switch (chart.type) {
          case 'pie':
            return (
              <div key={index} className="events-statistics-panel__item">
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
              <div key={index} className="events-statistics-panel__item">
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
              <div key={index} className="events-statistics-panel__item">
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

export default EventsStatisticsPanel;
