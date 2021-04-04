import React from "react";
import EventList from "./EventList";
import "./EventView.scss";

const EventView = () => {
    return (
        <div className="event-view">
            <EventList/>
        </div>
    );
};

export default EventView;