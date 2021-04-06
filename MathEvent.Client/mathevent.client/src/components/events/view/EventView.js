import React from "react";
import { useSelector } from "react-redux";
import EventInfo from "./EventInfo";
import EventGrid from "./EventGrid";
import EventList from "./EventList";
import "./EventView.scss";

const EventView = () => {
    const { isGridView, eventInfo } = useSelector(state => state.event);

    return (
        <div className="event-view">
            {isGridView
                ? (<EventGrid/>)
                : (<EventList/>)
            }
            <EventInfo event={eventInfo}/>
        </div>
    );
};

export default EventView;