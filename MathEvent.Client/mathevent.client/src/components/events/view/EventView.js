import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useTitle } from '../../../hooks';
import {
  fetchEvents,
  fetchEventBreadcrumbs,
} from '../../../store/actions/event';
import EventGrid from './EventGrid';
import EventInfo from './EventInfo';
import EventList from './EventList';
import './EventView.scss';

const EventView = () => {
  const dispatch = useDispatch();
  const { eventInfo, isGridView } = useSelector((state) => state.event);

  useEffect(() => {
    if (eventInfo) {
      if (eventInfo.hierarchy) {
        dispatch(fetchEventBreadcrumbs(eventInfo.id));
        dispatch(fetchEvents(eventInfo.id));
      } else {
        const { parentId } = eventInfo;
        dispatch(fetchEvents(parentId));
        dispatch(fetchEventBreadcrumbs(parentId));
      }
    } else {
      dispatch(fetchEvents(null));
      dispatch(fetchEventBreadcrumbs(null));
    }
  }, [dispatch, eventInfo]);

  useTitle('События');

  return (
    <div className="event-view">
      {isGridView ? <EventGrid /> : <EventList />}
      <EventInfo />
    </div>
  );
};

export default EventView;

// import React from 'react';
// import { useSelector } from 'react-redux';
// import { useTitle } from '../../../hooks';
// import Loader from '../../_common/Loader';
// import EventGrid from './EventGrid';
// import EventInfo from './EventInfo';
// import EventList from './EventList';
// import './EventView.scss';

// const EventView = () => {
//   const { isFetchingEvent, isFetchingEvents, isGridView } = useSelector(
//     (state) => state.event
//   );

//   useTitle('События');

//   return (
//     <div className="event-view">
//       {isFetchingEvents ? (
//         <Loader className="event-view__loader" size="medium" />
//       ) : isGridView ? (
//         <EventGrid />
//       ) : (
//         <EventList />
//       )}
//       {isFetchingEvent ? (
//         <Loader className="event-view__loader" size="medium" />
//       ) : (
//         <EventInfo />
//       )}
//     </div>
//   );
// };

// export default EventView;
