import React from "react";
import { useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import { Date } from "../../_common/Date";
import EventFiles from "./EventFiles";
import Image from "../../_common/Image";
import Loader from "../../_common/Loader";

const EventInfo = () => {
    const { eventInfo, isFetchingEvent } = useSelector(state => state.event);

    if (!eventInfo) {
        return null;
    }

    return (
        isFetchingEvent
            ? (<Loader className="event-grid__loader" size="medium"/>)
            : (
                <Paper className="event-info">
                    <section className="event-info__info">
                        <section className="event-info__info--image">
                            <Image
                                className="event-info__image"
                                src="https://vancouverhumanesociety.bc.ca/wp-content/uploads/2019/01/Upcoming-eventsiStock-978975308-e1564610924151-1024x627.jpg"
                                alt={eventInfo.name}/>
                        </section>
                        <section className="event-info__info--desctirption">
                            <h4>{eventInfo.name}</h4>
                            <Date
                                primaryText="Дата начала:"
                                date={eventInfo.startDate}/>
                            <h5>{`Описание: ${eventInfo.description}`}</h5>
                            <h6>{`Организация: ${eventInfo.organization ? eventInfo.organization.name : "отсутствует"}`}</h6>
                        </section>
                    </section>
                    <section className="event-info__files">
                        <EventFiles/>
                    </section>
                </Paper>
            )
    );
};

export default EventInfo;