import React from "react";
import { useSelector } from "react-redux";
import EventBreadcrumbs from "./EventBreadcrumbs";
import EventHeader from "./EventHeader";
import EventListItem from "./EventListItem";
import Loader from "../../_common/Loader";

const EventList = () => {
    const { events, selectedEvent, isFetchingEvents } = useSelector(state => state.event);

    return (
        <div className="event-list">
            <EventHeader headerText="Список событий"/>
            <EventBreadcrumbs/>
            {isFetchingEvents
                ? (<Loader className="event-list__loader" size="medium"/>)
                : (
                    <ul className="event-list__items">
                        {
                            events.map((event, index) => (
                                <EventListItem
                                    key={event.id}
                                    event={event}
                                    isSelected={selectedEvent && event.id === selectedEvent.id}
                                    index={index + 1}
                                />
                            ))
                        }
                    </ul>
                )}
        </div>
    );
};

export default EventList;