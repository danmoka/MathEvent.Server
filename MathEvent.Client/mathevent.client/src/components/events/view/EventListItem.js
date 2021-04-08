import React, { useCallback } from "react";
import { useDispatch } from "react-redux";
import { selectEvent, fetchEvent, fetchEvents, fetchBreadcrumbs } from "../../../store/actions/event";
import ListItem from "../../_common/ListItem";

const EventListItem = ({ event, isSelected, index }) => {
    const dispatch = useDispatch();

    const handleClick = useCallback(() => {
        dispatch(selectEvent(event));
        dispatch(fetchEvent(event.id));

        if (event.hierarchy) {
            dispatch(fetchBreadcrumbs(event.id));
            dispatch(fetchEvents(event.id));
        }

    }, [dispatch, event]);

    return (
        <ListItem
            primaryText={event.name}
            secondaryText={event.startDate}
            isSelected={isSelected}
            index={index}
            onClick={handleClick}
        />
    );
};

export default EventListItem;