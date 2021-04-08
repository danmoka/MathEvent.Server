import React, { useCallback } from "react";
import { useDispatch } from "react-redux";
import { selectEvent, fetchEvents, fetchEvent, fetchBreadcrumbs } from "../../../store/actions/event";
import GridCard from "../../_common/GridCard";

const EventGridCard = ({ event, isSelected, index }) => {
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
        <GridCard
            primaryText={event.name}
            secondaryText={event.startDate}
            additionalInfo={event.description}
            isSelected={isSelected}
            index={index}
            onClick={handleClick}
        />
    );
};

export default EventGridCard;