import React, { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents, fetchEventBreadcrumbs } from "../../../store/actions/event";
import CommonBreadcrumbs from "../../_common/Breadcrumbs";
import Loader from "../../_common/Loader";

const prepareCrumbs = (crumbs, onClick) =>
    crumbs.map((crumb, index) => ({
        id: crumb.id,
        primaryText: crumb.name,
        index: index + 1,
        isLast: index === (crumbs.length - 1),
        onClick: () => onClick(crumb)
    }));

const EventBreadcrumbs = () => {
    const dispatch = useDispatch();
    let { crumbs, isFetchingEventBreadcrumbs  } = useSelector(state => state.event);
    crumbs = [ {id: null, name: "Корень"}, ...crumbs ];

    const handleCrumbClick = useCallback((crumb) => {
        dispatch(fetchEvents(crumb.id));
        dispatch(fetchEventBreadcrumbs(crumb.id));
    });

    const handleBackButtonClick = useCallback(() => {
        const lastCrumb = crumbs[crumbs.length - 2];
        dispatch(fetchEvents(lastCrumb ? lastCrumb.id : null));
        dispatch(fetchEventBreadcrumbs(lastCrumb ? lastCrumb.id : null));
    });

    const preparedCrumbs = prepareCrumbs(
        crumbs,
        handleCrumbClick
    );

    return (
        isFetchingEventBreadcrumbs
            ? (<Loader className="event-grid__loader" size="medium"/>)
            : (<CommonBreadcrumbs items={preparedCrumbs} backButtonOnClick={handleBackButtonClick}/>)
    );
};

export default EventBreadcrumbs;