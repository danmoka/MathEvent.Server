import React, { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { selectEvent, fetchEvent, fetchEvents, fetchBreadcrumbs } from "../../../store/actions/event";
import EventBreadcrumbs from "./EventBreadcrumbs";
import EventHeader from "./EventHeader";
import CommonGrid from "../../_common/Grid";
import Loader from "../../_common/Loader";

const prepareEvents = (events, selectedEvent, onClick) =>
    events.map((event, index) => ({
        id: event.id,
        primaryText: event.name,
        secondaryText: event.startDate,
        isSelected: selectedEvent && event.id === selectedEvent.id,
        index: index + 1,
        onClick: () => onClick(event)
}));

const EventGrid = () => {
    const dispatch = useDispatch();
    const { events, selectedEvent, isFetchingEvents } = useSelector(state => state.event);
    const handleEventClick = useCallback((event) => {
        dispatch(selectEvent(event));
        dispatch(fetchEvent(event.id));

        if (event.hierarchy) {
            dispatch(fetchBreadcrumbs(event.id));
            dispatch(fetchEvents(event.id));
        }
    }, []);
    const preparedEvents = prepareEvents(
        events,
        selectedEvent,
        handleEventClick
    );

    return (
        <div className="event-grid">
            <EventHeader headerText="Карточки событий"/>
            <EventBreadcrumbs/>
            {isFetchingEvents
                ? (<Loader className="event-grid__loader" size="medium"/>)
                : (
                    <div className="event-grid__items">
                        <CommonGrid items={preparedEvents}/>
                    </div>
                )}
        </div>
    );
};

export default EventGrid;