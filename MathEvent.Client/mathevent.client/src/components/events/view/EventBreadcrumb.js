import React, { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents, fetchBreadcrumbs } from "../../../store/actions/event";
import Breadcrumb from "../../_common/Breadcrumb";
import Loader from "../../_common/Loader";

const EventBreadcrumb = ({ crumb, isLast }) => {
    const { isFetchingBreadcrumbs } = useSelector(state => state.event);
    const dispatch = useDispatch();

    const handleClick = useCallback(() => {
        dispatch(fetchEvents(crumb.id));
        dispatch(fetchBreadcrumbs(crumb.id));

    }, [dispatch, crumb]);

    return (
        isFetchingBreadcrumbs
            ? (<Loader className="event-grid__loader" size="medium"/>)
            : (
                <Breadcrumb
                    crumb={crumb}
                    isLast={isLast}
                    onClick={handleClick}/>
            )
    );
};

export default EventBreadcrumb;