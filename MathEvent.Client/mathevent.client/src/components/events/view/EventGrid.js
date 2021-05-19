import React, { useCallback } from 'react';
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
  showCreateEventModal,
  showDeleteEventModal,
} from '../../../store/actions/event';
import { IconButton, iconTypes } from '../../_common/Icon';
import { navigateToEventEdit } from '../../../utils/navigator';
import { getImageSrc } from '../../../utils/get-image-src';
import EventBreadcrumbs from './EventBreadcrumbs';
import CommonGrid from '../../_common/Grid';
import Loader from '../../_common/Loader';
import images from '../../../constants/images';

const prepareEvents = (
  events,
  selectedEvent,
  onEventEdit,
  onEventDelete,
  onClick,
  isDarkTheme
) =>
  events.map((event, index) => ({
    id: event.id,
    primaryText: event.name,
    secondaryText: moment(event.startDate).format('LL'),
    additionalInfo: event.description,
    image: event.avatarPath
      ? getImageSrc(event.avatarPath)
      : isDarkTheme
      ? images.eventDefaultDark
      : images.eventDefault,
    isSelected: selectedEvent && event.id === selectedEvent.id,
    index: index + 1,
    onClick: () => onClick(event),
    actions: [
      {
        id: 'edit',
        label: 'Редактировать',
        icon: iconTypes.edit,
        onClick: () => onEventEdit(event),
      },
      {
        id: 'delete',
        label: 'Удалить',
        icon: iconTypes.delete,
        onClick: () => onEventDelete(event),
      },
    ],
  }));

const EventGrid = () => {
  const dispatch = useDispatch();
  const { events, selectedEvent, isFetchingEvents } = useSelector(
    (state) => state.event
  );
  const { isDarkTheme } = useSelector((state) => state.app);

  const handleEventClick = useCallback((event) => {
    dispatch(selectEvent(event));
    dispatch(fetchEvent(event.id));

    if (event.hierarchy) {
      dispatch(fetchEventBreadcrumbs(event.id));
      dispatch(fetchEvents(event.id));
    }
  }, []);

  const handleEventEdit = useCallback((event) => {
    dispatch(selectEvent(event));
    navigateToEventEdit(event.id);
  });

  const handleEventDelete = useCallback(
    (event) => {
      dispatch(showDeleteEventModal({ event }));
    },
    [dispatch]
  );

  const handleEventCreate = () => dispatch(showCreateEventModal());

  const preparedEvents = prepareEvents(
    events,
    selectedEvent,
    handleEventEdit,
    handleEventDelete,
    handleEventClick,
    isDarkTheme
  );

  return (
    <div className="event-grid">
      <Paper className="event-grid__header">
        <Typography variant="h6" gutterBottom>
          События
        </Typography>
        <IconButton type={iconTypes.add} onClick={handleEventCreate} />
      </Paper>
      <EventBreadcrumbs />
      {isFetchingEvents ? (
        <Loader className="event-grid__loader" size="medium" />
      ) : (
        <div className="event-grid__items">
          <CommonGrid items={preparedEvents} />
        </div>
      )}
    </div>
  );
};

export default EventGrid;
