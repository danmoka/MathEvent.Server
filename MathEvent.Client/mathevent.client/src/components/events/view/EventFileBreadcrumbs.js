import React, { useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchFiles, fetchFileBreadcrumbs } from '../../../store/actions/file';
import CommonBreadcrumbs from '../../_common/Breadcrumbs';
import Loader from '../../_common/Loader';

const prepareCrumbs = (crumbs, onClick) =>
  crumbs.map((crumb, index) => ({
    id: crumb.id,
    primaryText: crumb.name,
    index: index + 1,
    isLast: index === crumbs.length - 1,
    onClick: () => onClick(crumb),
  }));

const EventFileBreadcrumbs = () => {
  const dispatch = useDispatch();
  const { eventInfo } = useSelector((state) => state.event);
  let { crumbs } = useSelector((state) => state.file);
  const { isFetchingFileBreadcrumbs } = useSelector((state) => state.file);
  crumbs = [{ id: null, name: 'Корень' }, ...crumbs];

  const handleCrumbClick = useCallback((crumb) => {
    dispatch(fetchFiles({ fileId: crumb.id, ownerId: eventInfo.ownerId }));
    dispatch(fetchFileBreadcrumbs(crumb.id));
  });

  const handleBackButtonClick = useCallback(() => {
    const lastCrumb = crumbs[crumbs.length - 2];
    dispatch(
      fetchFiles({
        fileId: lastCrumb ? lastCrumb.id : null,
        ownerId: eventInfo.ownerId,
      })
    );
    dispatch(fetchFileBreadcrumbs(lastCrumb ? lastCrumb.id : null));
  });

  const preparedCrumbs = prepareCrumbs(crumbs, handleCrumbClick);

  return isFetchingFileBreadcrumbs ? (
    <Loader className="event-grid__loader" size="medium" />
  ) : (
    <CommonBreadcrumbs
      items={preparedCrumbs}
      backButtonOnClick={handleBackButtonClick}
    />
  );
};

export default EventFileBreadcrumbs;
