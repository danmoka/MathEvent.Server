import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useParams } from "react-router";
import { fetchEvent } from "../../../store/actions/event";
import EventEditInfo from "./EventEditInfo";
import EventManagerList from "./EventManagerList";
import Loader from "../../_common/Loader";
import "./EventEdit.scss";

const EventEdit = () => {
    const dispatch = useDispatch();
    const { id } = useParams();
    const { eventInfo, isFetchingEvent } = useSelector((state) => state.event);

    useEffect(() => {
        if (!eventInfo || eventInfo.id !== id) {
            dispatch(fetchEvent(id));
        }
    }, [dispatch, id]);

    return (
        isFetchingEvent || !eventInfo
        ? (<Loader className="event-edit__loader" size="medium"/>)
        : (
            <div className="event-edit">
                <EventManagerList/>
                <EventEditInfo/>
            </div>
        )
    );
};

export default EventEdit;