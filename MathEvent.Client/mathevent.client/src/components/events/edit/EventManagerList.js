import React, { useCallback, useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import { fetchUsers } from "../../../store/actions/user";
import { showEditManagersEventModal } from "../../../store/actions/event";
import { IconButton, iconTypes } from "../../_common/Icon";
import List from "../../_common/List";

const prepareNewManagers = (users, managers) =>
    users.map((user, index) => ({
        id: user.id,
        checked: managers.filter((manager) => manager.id === user.id).length > 0,
        primaryText: `${user.name} ${user.surname}`,
        secondaryText: user.userName,
        index: index + 1
}));

const prepareManagers = (managers) =>
    managers.map((manager, index) => ({
        id: manager.id,
        primaryText: `${manager.name} ${manager.surname}`,
        secondaryText: manager.userName,
        avatarText: `${manager.name.slice(0, 1)}${manager.surname.slice(0, 1)}`,
        index: index + 1,
        onClick: () => {},
}));

const EventManagerList = () => {
    const dispatch = useDispatch();
    const { eventInfo: event } = useSelector((state) => state.event);
    const { users } = useSelector((state) => state.user);

    const [eventId, setEventId] = useState(null);
    const [managers, setManagers] = useState([]);

    useEffect(() => {
        dispatch(fetchUsers())
    }, [dispatch]);

    useEffect(() => {
        if (event) {
            setEventId(event.id);
            setManagers(event.managers);
        }
    }, [event?.id, event?.managers]);

    const preparedManagers = prepareManagers(
        managers
    );

    const preparedNewManagers = prepareNewManagers(users, managers);

    const handleManagerEdit = useCallback(
        () => {
            dispatch(showEditManagersEventModal({ event, preparedNewManagers }));
        },
        [dispatch, event, preparedNewManagers]
    );

    return (
        <Paper className="event-manager-list">
            <div className="event-manager-list__header">
                <Typography variant="h5" gutterBottom>Менеджеры</Typography>
                <IconButton
                    type={iconTypes.edit}
                    onClick={handleManagerEdit}
                />
            </div>
            <div className="event-manager-list__items">
                <List className="event-list__ul" items={preparedManagers}/>
            </div>
        </Paper>
    );
};

export default EventManagerList;