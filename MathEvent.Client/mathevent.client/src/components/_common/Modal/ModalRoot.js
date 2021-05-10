import React from "react";
import { useSelector } from "react-redux";
import EditManagersEventModal from "../../events/edit/EditManagersEventModal";
import EventLocationModal from "../../events/view/EventLocationModal";
import EventStatisticsModal from "../../statistics/event/EventStatisticsModal";
import CreateEventModal from "../../events/view/CreateEventModal";
import CreateFolderModal from "../../files/view/CreateFolderModal";
import DeleteEventModal from "../../events/view/DeleteEventModal";
import DeleteFileModal from "../../files/view/DeleteFileModal";
import LogoutModal from "../../_app/LogoutModal";
import UploadEventAvatarModal from "../../events/edit/UploadEventAvatar";
import UploadFilesModal from "../../files/view/UploadFilesModal";
import modalTypes from "../../../constants/modal-types";

const modals = {
    [modalTypes.createEvent]: CreateEventModal,
    [modalTypes.deleteEvent]: DeleteEventModal,
    [modalTypes.uploadEventAvatar]: UploadEventAvatarModal,
    [modalTypes.editManagersEventModal]: EditManagersEventModal,
    [modalTypes.eventLocation]: EventLocationModal,
    [modalTypes.eventStatistics]: EventStatisticsModal,
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