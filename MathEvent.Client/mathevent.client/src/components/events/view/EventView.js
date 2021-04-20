import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents, fetchEventBreadcrumbs } from "../../../store/actions/event";
import { useTitle } from "../../../hooks";
import EventGrid from "./EventGrid";
import EventInfo from "./EventInfo";
import EventList from "./EventList";
import "./EventView.scss";

const EventView = () => {
    const dispatch = useDispatch();
    const { isGridView } = useSelector(state => state.event);

    useEffect(() => {
        dispatch(fetchEvents());
        dispatch(fetchEventBreadcrumbs());
    }, []);

    useTitle("События");

    return (
        <div className="event-view">
            {isGridView
                ? (<EventGrid/>)
                : (<EventList/>)
            }
            <EventInfo/>
        </div>
    );
};

export default EventView;