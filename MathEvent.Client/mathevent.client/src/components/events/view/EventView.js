import React from "react";
import { useSelector } from "react-redux";
import EventList from "./EventList";
import EventGrid from "./EventGrid";
import "./EventView.scss";

const EventView = () => {
    const { isGridView } = useSelector(state => state.event);

    return (
        <div className="event-view">
            {isGridView
                ? (<EventGrid/>)
                : (<EventList/>)
            }
        </div>
    );
};

export default EventView;