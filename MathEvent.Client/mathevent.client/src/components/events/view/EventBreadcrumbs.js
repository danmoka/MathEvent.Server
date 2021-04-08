import React from "react";
import { useSelector } from "react-redux";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import EventBreadcrumb from "./EventBreadcrumb";

const EventBreadcrumbs = () => {
    let { crumbs } = useSelector(state => state.event);
    crumbs = [ {id: null, name: "Корень"}, ...crumbs ]

    return (
        <Breadcrumbs 
            className="event-breadcrumbs">
            {crumbs.map((crumb, index) => (
                <EventBreadcrumb
                    key={crumb.id}
                    crumb={crumb}
                    isLast={index === (crumbs.length - 1)}
                />))}
        </Breadcrumbs>
    );
};

export default EventBreadcrumbs;