import React, { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents, fetchBreadcrumbs } from "../../../store/actions/event";
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
    let { crumbs, isFetchingBreadcrumbs  } = useSelector(state => state.event);
    crumbs = [ {id: null, name: "Корень"}, ...crumbs ]
    const handleCrumbClick = useCallback((crumb) => {
        dispatch(fetchEvents(crumb.id));
        dispatch(fetchBreadcrumbs(crumb.id));
    }, []);
    const preparedCrumbs = prepareCrumbs(
        crumbs,
        handleCrumbClick
    );

    return (
        isFetchingBreadcrumbs
            ? (<Loader className="event-grid__loader" size="medium"/>)
            : (
                <div className="event-breadcrumbs">
                    <CommonBreadcrumbs items={preparedCrumbs}/>
                </div>
            )
    );
};

export default EventBreadcrumbs;