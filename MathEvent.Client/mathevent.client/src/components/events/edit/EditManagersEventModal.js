import React, { useCallback, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { hideModal } from "../../../store/actions/modal";
import { patchEvent } from "../../../store/actions/event";
import { CreateModal } from "../../_common/Modal";
import List from "../../_common/List";

const EditManagersEventModal = () => {
    const dispatch = useDispatch();
    const { event, preparedNewManagers } = useSelector((state) => state.modal.modalProps);
    const [checkedItemIds, setCheckedItemIds] = useState(
        preparedNewManagers.filter((manager) => manager.checked).map((manager) => manager.id)
    );

    const handleCheck =  useCallback((newCheckedItemIds) => {
        setCheckedItemIds(newCheckedItemIds);
    });

    const handlePatchEvent = useCallback(
        (data) => {
            dispatch(
                patchEvent({
                    eventId: event.id,
                    data: data,
                })
              );
        },
        [dispatch, event.id]
    );

    const handleSave = useCallback(() => {
        handlePatchEvent([
            {
                value: checkedItemIds,
                path: "/Managers",
                op: "replace"
            }
        ]);
        dispatch(hideModal());
    }, [handlePatchEvent, checkedItemIds]);

    return (
        <CreateModal title="Менеджеры события" createButtonText = "Готово" onCreate={handleSave}>
            <List className="event-list__ul" items={preparedNewManagers} onCheck={handleCheck}/>
        </CreateModal>
    );
};

export default EditManagersEventModal;