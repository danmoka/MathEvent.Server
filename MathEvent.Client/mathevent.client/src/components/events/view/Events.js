import React, { useEffect } from "react";
import { Route } from "react-router";
import { useDispatch } from "react-redux";
import { fetchEvents } from "../../../store/actions/event";
import EventView from "./EventView";
import routes from "../../../utils/routes";

const Events = () => {
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(fetchEvents());
    }, [dispatch]);

    return (
        <>
            <Route
                path={routes.events}
                exact
                render={(props) => <EventView {...props}/>}
            />
        </>
    );
};

export default Events;