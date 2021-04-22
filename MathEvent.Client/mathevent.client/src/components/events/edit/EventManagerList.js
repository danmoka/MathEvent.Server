import React, { useCallback, useState, useEffect } from "react";
import { useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import { iconTypes } from "../../_common/Icon";
import List from "../../_common/List";

const prepareManagers = (managers, onManagerDelete, onClick) =>
    managers.map((manager, index) => ({
        id: manager.id,
        primaryText: `${manager.name} ${manager.surname}`,
        secondaryText: manager.userName,
        index: index + 1,
        onClick: () => onClick(manager),
        actions: [
            {
                id: "delete",
                label: "Удалить",
                icon: iconTypes.delete,
                onClick: () => onManagerDelete(manager),
            }
        ]
    }));

const EventManagerList = () => {
    const { eventInfo: event } = useSelector((state) => state.event);

    const [eventId, setEventId] = useState(null);
    const [managers, setManagers] = useState([]);

    useEffect(() => {
        if (event) {
            setEventId(event.id);
            setManagers(event.managers);
        }
    }, [event?.id]);

    const handleManagerDelete = useCallback((manager) => {

    });

    const preparedManagers = prepareManagers(
        managers,
        handleManagerDelete
    )

    return (
        <Paper className="event-manager-list">
            <div className="event-manager-list__header">
            <Typography variant="h5" gutterBottom>Менеджеры</Typography>
            </div>
            <div className="event-manager-list__items">
                <List items={preparedManagers}/>
            </div>
        </Paper>
    );
};

export default EventManagerList;