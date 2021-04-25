import React from "react";
import { useDispatch, useSelector } from "react-redux";
import { deleteEvent } from "../../../store/actions/event";
import { DeleteModal } from "../../_common/Modal";

const DeleteEventModal = () => {
    const dispatch = useDispatch();
    const { event } = useSelector((state) => state.modal.modalProps);

    const handleEventDelete = () => dispatch(deleteEvent({ eventId: event.id}));

    return (
        <DeleteModal
            title={event.name}
            deleteText="Вы действительно хотите удалить событие? Сначала удалите все вложенные!"
            onDelete={handleEventDelete}
        />
    );
};

export default DeleteEventModal;