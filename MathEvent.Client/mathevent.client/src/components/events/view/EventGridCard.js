import React, { useCallback } from "react";
import { useDispatch } from "react-redux";
import GridCard from "../../_common/GridCard";
import { selectEvent } from "../../../store/actions/event";

const EventGridCard = ({ event, isSelected, index }) => {
    const dispatch = useDispatch();

    const handleClick = useCallback(() => {
        dispatch(selectEvent(event));
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