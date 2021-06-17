import React from "react";
import { Route } from "react-router-dom";
import EventEdit from "../edit/EventEdit";
import EventView from "./EventView";
import routes from "../../../utils/routes";

const Events = () => {
    return (
        <>
            <Route
                path={routes.events.main}
                exact
                render={(props) => <EventView {...props}/>}
            />
            <Route
                path={`${routes.events.edit(":id")}`}
                exact
                render={(props) => <EventEdit {...props}/>}
            />
        </>
    );
};

export default Events;