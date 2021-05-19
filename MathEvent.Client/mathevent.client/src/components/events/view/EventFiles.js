import React, { useCallback, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import {
  downloadFile,
  fetchFile,
  fetchFiles,
  fetchFileBreadcrumbs,
  showDeleteFileModal,
  showCreateFolderModal,
  showUploadFilesModal,
} from '../../../store/actions/file';
import { IconButton, iconTypes } from '../../_common/Icon';
import EventFileBreadcrumbs from './EventFileBreadcrumbs';
import Files from '../../_common/File/Files';
import Loader from '../../_common/Loader';

const prepareFiles = (files, onFileDownload, onFileDelete, onClick) => {
  const fileActions = (file) => [
    {
      id: 'download',
      label: 'Скачать',
      icon: iconTypes.download,
      onClick: () => onFileDownload(file),
    },
    {
      id: 'delete',
      label: 'Удалить',
      icon: iconTypes.delete,
      onClick: () => onFileDelete(file),
    },
  ];
  const folderActions = (file) => [
    {
      id: 'delete',
      label: 'Удалить',
      icon: iconTypes.delete,
      onClick: () => onFileDelete(file),
    },
  ];

  files = files.map((file, index) => ({
    id: file.id,
    name: file.name,
    ext: file.extension,
    hierarchy: file.hierarchy,
    index: index + 1,
    onClick: () => onClick(file),
    actions: file.hierarchy ? folderActions(file) : fileActions(file),
  }));

  return files;
};

const EventFiles = () => {
  const dispatch = useDispatch();
  const { eventInfo } = useSelector((state) => state.event);
  const { files, crumbs, isFetchingFiles } = useSelector((state) => state.file);

  useEffect(() => {
    if (eventInfo) {
      dispatch(fetchFiles({ fileId: null, ownerId: eventInfo.ownerId }));
      dispatch(fetchFileBreadcrumbs(null));
    }
  }, [dispatch, eventInfo]);

  const handleFileClick = useCallback((file) => {
    dispatch(fetchFile(file.id));

    if (file.hierarchy) {
      dispatch(fetchFileBreadcrumbs(file.id));
      dispatch(fetchFiles({ fileId: file.id, ownerId: eventInfo.ownerId }));
    }
  });

  const handleFileDelete = useCallback(
    (file) => {
      dispatch(showDeleteFileModal({ file }));
    },
    [dispatch]
  );

  const handleFileDownload = useCallback((file) => {
    dispatch(downloadFile(file.id));
  });

  const handleFolderCreate = useCallback(() => {
    dispatch(showCreateFolderModal({ owner: eventInfo, crumbs }));
  }, [dispatch, eventInfo, crumbs]);

  const handleFilesUpload = useCallback(() => {
    dispatch(showUploadFilesModal({ owner: eventInfo, crumbs }));
  }, [dispatch, eventInfo, crumbs]);

  const preparedFiles = prepareFiles(
    files,
    handleFileDownload,
    handleFileDelete,
    handleFileClick
  );

  return (
    <div className="event-files">
      <Paper className="event-files__header">
        <section className="event-files__header__section">
          <Typography variant="h5" gutterBottom>
            Материалы
          </Typography>
        </section>
        <section className="event-files__header__section">
          <IconButton type={iconTypes.upload} onClick={handleFilesUpload} />
          <IconButton type={iconTypes.add} onClick={handleFolderCreate} />
        </section>
      </Paper>
      <EventFileBreadcrumbs />
      {isFetchingFiles ? (
        <Loader className="event-files__loader" size="medium" />
      ) : (
        <div className="event-files__items">
          <Files items={preparedFiles} />
        </div>
      )}
    </div>
  );
};

export default EventFiles;
