import React, { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import { IconButton, iconTypes } from "../../_common/Icon";
import { navigateToEventEdit } from "../../../utils/navigator";
import { selectEvent, fetchEvent, fetchEvents, fetchEventBreadcrumbs, showCreateEventModal, showDeleteEventModal } from "../../../store/actions/event";
import EventBreadcrumbs from "./EventBreadcrumbs";
import List from "../../_common/List";
import Loader from "../../_common/Loader";

const prepareEvents = (events, selectedEvent, onEventEdit, onEventDelete, onClick) =>
    events.map((event, index) => ({
        id: event.id,
        primaryText: event.name,
        secondaryText: event.startDate,
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


const EventList = () => {
    const dispatch = useDispatch();
    const { events, selectedEvent, isFetchingEvents } = useSelector(state => state.event);

    const handleEventClick = useCallback((event) => {
        dispatch(selectEvent(event));
        dispatch(fetchEvent(event.id));

        if (event.hierarchy) {
            dispatch(fetchEventBreadcrumbs(event.id));
            dispatch(fetchEvents(event.id));
        }
    }, []);

    const handleEventEdit = useCallback((event) => {
        dispatch(selectEvent(event));
        navigateToEventEdit(event.id);
    });

    const handleEventDelete = useCallback(
        (event) => {
            dispatch(showDeleteEventModal({ event }));
        },
        [dispatch]
    );

    const handleEventCreate = () => dispatch(showCreateEventModal());

    const preparedEvents = prepareEvents(
        events,
        selectedEvent,
        handleEventEdit,
        handleEventDelete,
        handleEventClick
    );

    return (
        <div className="event-list">
            <Paper className="event-list__header">
                <Typography variant="h6" gutterBottom>События</Typography>
                <IconButton
                    type={iconTypes.add}
                    onClick={handleEventCreate}
                />
            </Paper>
            <EventBreadcrumbs/>
            {isFetchingEvents
                ? (<Loader className="event-list__loader" size="medium"/>)
                : (
                    <Paper className="event-list__items">
                        <List className="event-list__ul" items={preparedEvents}/>
                    </Paper>
                )}
        </div>
    );
};

export default EventList;