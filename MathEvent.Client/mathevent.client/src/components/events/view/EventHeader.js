import React, { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import Switch from "../../_common/Switch";
import { setGridView } from "../../../store/actions/event";

const EventHeader = ({ headerText }) => {
    const { isGridView } = useSelector(state => state.event);
    const dispatch = useDispatch();

    const handleChange = useCallback((isGridView) => {
        dispatch(setGridView(isGridView));
    }, [dispatch, isGridView]);

    return (
        <div className="event-grid__header">
            <p>{headerText}</p>
            <Switch label="Карточки" checked={isGridView} onChange={handleChange}/>
        </div>
    )
}

export default EventHeader;