import React, { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { selectEvent, fetchEvent, fetchEvents, fetchBreadcrumbs } from "../../../store/actions/event";
import EventBreadcrumbs from "./EventBreadcrumbs";
import EventHeader from "./EventHeader";
import List from "../../_common/List";
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


const EventList = () => {
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
        <div className="event-list">
            <EventHeader headerText="Список событий"/>
            <EventBreadcrumbs/>
            {isFetchingEvents
                ? (<Loader className="event-list__loader" size="medium"/>)
                : (
                    <div className="event-list__items">
                        <List items={preparedEvents}/>
                    </div>
                )}
        </div>
    );
};

export default EventList;