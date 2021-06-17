import React, { useCallback, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { uploadFiles } from "../../../store/actions/file";
import CreateModal from "../../_common/Modal/CreateModal";
import FileDropzone, { fileTypes } from "../../_common/FileDropzone";

const UploadFilesModal = () => {
    const dispatch = useDispatch();
    const { owner, crumbs } = useSelector((state) => state.modal.modalProps);
    const [files, setFiles] = useState([]);
    const parent = crumbs && crumbs.length > 0 ? crumbs[crumbs.length - 1] : null;

    const handleFilesDrop = useCallback((newfiles) => {
        if (newfiles.length > 0) setFiles(...files, newfiles);
    }, []);

    const handleFilesUpload = useCallback(() => {
        if (files.length) {
            dispatch(uploadFiles({
                fileId: parent ? parent.id : null,
                ownerId: owner.ownerId,
                files: files
            }));
        }
    }, [dispatch, files, owner, crumbs]);

    return (
        <CreateModal title="Загрузка файлов" createButtonText="Загрузить" onCreate={handleFilesUpload}>
          <FileDropzone
            acceptedFileTypes={[fileTypes.image.types, fileTypes.application.types, fileTypes.text.types]}
            acceptedFileValues={[fileTypes.image.values, fileTypes.application.values, fileTypes.text.values]}
            onDrop={handleFilesDrop} />
        </CreateModal>
      );
};

export default UploadFilesModal;