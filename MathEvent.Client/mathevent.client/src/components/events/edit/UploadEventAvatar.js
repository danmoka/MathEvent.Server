import React, { useCallback, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { uploadEventAvatar } from "../../../store/actions/event";
import CreateModal from "../../_common/Modal/CreateModal";
import FileDropzone, { fileTypes } from "../../_common/FileDropzone";

const UploadEventAvatarModal = () => {
    const dispatch = useDispatch();
    const { eventId } = useSelector((state) => state.modal.modalProps);
    const [file, setFile] = useState(null);

    const handleFilesDrop = useCallback((newfiles) => {
        if (newfiles.length > 0) setFile(newfiles[0]);
    }, []);

    const handleEventAvatarUpload = useCallback(() => {
        if (file) {
            dispatch(uploadEventAvatar({
                eventId: eventId,
                file: file
            }));
        }
    }, [dispatch, file, eventId]);

    return (
        <CreateModal title="Загрузка изображения" createButtonText="Загрузить" onCreate={handleEventAvatarUpload}>
          <FileDropzone
            acceptedFileTypes={[fileTypes.image.types]}
            acceptedFileValues={[fileTypes.image.values]}
            maxFiles={1}
            onDrop={handleFilesDrop} />
        </CreateModal>
      );
};

export default UploadEventAvatarModal;