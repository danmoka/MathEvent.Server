import React from "react";
import { useSelector } from "react-redux";
import EditManagersEventModal from "../../events/edit/EditManagersEventModal";
import CreateEventModal from "../../events/view/CreateEventModal";
import CreateFolderModal from "../../files/view/CreateFolderModal";
import DeleteEventModal from "../../events/view/DeleteEventModal";
import DeleteFileModal from "../../files/view/DeleteFileModal";
import LogoutModal from "../../_app/LogoutModal";
import UploadFilesModal from "../../files/view/UploadFilesModal";
import modalTypes from "../../../constants/modal-types";

const modals = {
    [modalTypes.createEvent]: CreateEventModal,
    [modalTypes.deleteEvent]: DeleteEventModal,
    [modalTypes.editManagersEventModal]: EditManagersEventModal,
    [modalTypes.createFolder]: CreateFolderModal,
    [modalTypes.deleteFile]: DeleteFileModal,
    [modalTypes.uploadFiles]: UploadFilesModal,
    [modalTypes.logout]: LogoutModal
};

const ModalRoot = () => {
    const { modalType, modalProps } = useSelector((state) => state.modal);

    if (!modalType) {
        return null;
    }

    const SpecificModal = modals[modalType];

    return <SpecificModal {...modalProps}/>
};

export default ModalRoot;