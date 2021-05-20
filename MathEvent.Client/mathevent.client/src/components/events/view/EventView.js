import React from 'react';
import { useSelector } from 'react-redux';
import { useTitle } from '../../../hooks';
import EventGrid from './EventGrid';
import EventInfo from './EventInfo';
import EventList from './EventList';
import './EventView.scss';

const EventView = () => {
  const { isGridView } = useSelector((state) => state.event);

  useTitle('События');

  return (
    <div className="event-view">
      {isGridView ? <EventGrid /> : <EventList />}
      <EventInfo />
    </div>
  );
};

export default EventView;
