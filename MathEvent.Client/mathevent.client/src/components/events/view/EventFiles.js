import React, { useCallback, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import Typography from "@material-ui/core/Typography";
import { fetchFile, fetchFiles, fetchFileBreadcrumbs } from "../../../store/actions/file";
import { iconTypes } from "../../_common/Icon";
import EventFileBreadcrumbs from "./EventFileBreadcrumbs";
import Files from "../../_common/File/Files";
import Loader from "../../_common/Loader";

const prepareFiles = (files, onFileDownload, onFileEdit, onFileDelete, onClick) =>
    files.map((file, index) => ({
        id: file.id,
        name: file.name,
        ext: file.extension,
        hierarchy: file.hierarchy,
        index: index + 1,
        onClick: () => onClick(file),
        actions: [
            {
                id: "download",
                label: "Скачать",
                icon: iconTypes.download,
                onClick: () => onFileDownload(file),
            },
            {
                id: "edit",
                label: "Редактировать",
                icon: iconTypes.edit,
                onClick: () => onFileEdit(file),
            },
            {
                id: "delete",
                label: "Удалить",
                icon: iconTypes.delete,
                onClick: () => onFileDelete(file),
            }
        ]
}));

const EventFiles = () => {
    const dispatch = useDispatch();
    const { eventInfo } = useSelector(state => state.event);
    const { files, isFetchingFiles } = useSelector(state => state.file);

    useEffect(() => {
        dispatch(fetchFiles({fileId: null, ownerId: eventInfo.ownerId}));
    }, [dispatch, eventInfo]);

    const handleFileClick = useCallback((file) => {
        dispatch(fetchFile(file.id));

        if (file.hierarchy) {
            dispatch(fetchFileBreadcrumbs(file.id));
            dispatch(fetchFiles({fileId: file.id, ownerId: eventInfo.ownerId}));
        }
    });

    const handleFileEdit = useCallback((file) => {

    });

    const handleFileDelete = useCallback((file) => {

    });

    const handleFileDownload = useCallback((file) => {

    });

    const preparedFiles = prepareFiles(
        files,
        handleFileDownload,
        handleFileEdit,
        handleFileDelete,
        handleFileClick
    );

    return (
        <div className="event-files">
            <div className="event-files__header">
                <Typography variant="h5" gutterBottom>Материалы</Typography>
            </div>
            <EventFileBreadcrumbs/>
            {isFetchingFiles
                ? (<Loader size="medium"/>)
                : (<Files items={preparedFiles}/>)}
        </div>
    );
};

export default EventFiles;