import React from "react";
import { useDispatch, useSelector } from "react-redux";
import { deleteFile } from "../../../store/actions/file";
import { DeleteModal } from "../../_common/Modal";

const DeleteFileModal = () => {
    const dispatch = useDispatch();
    const { file } = useSelector((state) => state.modal.modalProps);

    const handleFileDelete = () => dispatch(deleteFile({ fileId: file.id}));

    return (
        <DeleteModal
            title={file.name}
            deleteText={`Вы действительно хотите удалить ${file.hierarchy ? 'папку' : 'файл'}? Сначала удалите все вложенные файлы и папки!`}
            onDelete={handleFileDelete}
        />
    );
};

export default DeleteFileModal;