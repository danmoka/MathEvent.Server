import React, { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { selectEvent, fetchEvent, fetchEvents, fetchBreadcrumbs, setGridView } from "../../../store/actions/event";
import { iconTypes } from "../../_common/Icon";
import { navigateToEventEdit } from "../../../utils/navigator";
import EventBreadcrumbs from "./EventBreadcrumbs";
import CommonGrid from "../../_common/Grid";
import Loader from "../../_common/Loader";
import Switch from "../../_common/Switch";

const prepareEvents = (events, selectedEvent, onEventEdit, onEventDelete, onClick) =>
    events.map((event, index) => ({
        id: event.id,
        primaryText: event.name,
        secondaryText: event.startDate,
        additionalInfo: event.description,
        isSelected: selectedEvent && event.id === selectedEvent.id,
        index: index + 1,
        onClick: () => onClick(event),
        actions: [
            {
                id: "edit",
                label: "Редактировать",
                icon: iconTypes.edit,
                onClick: () => onEventEdit(event),
            },
            {
                id: "delete",
                label: "Удалить",
                icon: iconTypes.delete,
                onClick: () => onEventDelete(event),
            }
        ]
}));

const EventGrid = () => {
    const dispatch = useDispatch();
    const { events, selectedEvent, isFetchingEvents, isGridView } = useSelector(state => state.event);

    const handleEventClick = useCallback((event) => {
        dispatch(selectEvent(event));
        dispatch(fetchEvent(event.id));

        if (event.hierarchy) {
            dispatch(fetchBreadcrumbs(event.id));
            dispatch(fetchEvents(event.id));
        }
    }, []);

    const handleEventEdit = useCallback((event) => {
        dispatch(selectEvent(event));
        navigateToEventEdit(event.id);
    });

    const handleEventDelete = useCallback((event) => {

    });

    const preparedEvents = prepareEvents(
        events,
        selectedEvent,
        handleEventEdit,
        handleEventDelete,
        handleEventClick
    );

    const handleViewChange = useCallback((isGridView) => {
        dispatch(setGridView(isGridView));
    }, [dispatch, isGridView]);

    return (
        <div className="event-grid">
            <div className="event-grid__header">
                <p>Карточки событий</p>
                <Switch label="Карточки" checked={isGridView} onChange={handleViewChange}/>
            </div>
            <EventBreadcrumbs/>
            {isFetchingEvents
                ? (<Loader className="event-grid__loader" size="medium"/>)
                : (
                    <div className="event-grid__items">
                        <CommonGrid items={preparedEvents}/>
                    </div>
                )}
        </div>
    );
};

export default EventGrid;