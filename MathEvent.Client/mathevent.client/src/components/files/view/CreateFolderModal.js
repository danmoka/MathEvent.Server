import React, { useCallback, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { createFile } from "../../../store/actions/file";
import { CreateModal, modalSizes } from "../../_common/Modal";
import TextField from "../../_common/TextField";

const CreateFolderModal = () => {
    const dispatch = useDispatch();
    const { owner, crumbs } = useSelector((state) => state.modal.modalProps);
    const parent = crumbs && crumbs.length > 0 ? crumbs[crumbs.length - 1] : null;

    const [name, setName] = useState("");

    const handleNameValueChange = useCallback((newName) => {
        setName(newName);
    }, []);

    const handleCreate = useCallback(() => {
        const file = {
            "Name": name,
            "ParentId": parent ? parent.id : null,
            "Hierarchy": true,
            "OwnerId": owner.ownerId
        };

        dispatch(createFile({ file }));
      }, [ dispatch, name]);

    return (
        <CreateModal title="Новая папка" size={modalSizes.small} onCreate={handleCreate}>
            <TextField
                className="event-form__control"
                label="Название"
                value={name}
                onChange={handleNameValueChange}
                />
        </CreateModal>
    );
};

export default CreateFolderModal;