import React from "react";
import { useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import { Date } from "../../_common/Date";
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
                    <section className="event-info__section--description">
                        <h4>{eventInfo.name}</h4>
                        <hr/>
                        <Date
                            primaryText="Дата начала:"
                            date={eventInfo.startDate}/>
                        <h5>{`Описание: ${eventInfo.description}`}</h5>
                        <h6>{`Организация: ${eventInfo.organization ? eventInfo.organization.name : "отсутствует"}`}
                        </h6>
                    </section>
                    <section className="event-info__section--image">
                        <img
                            className="event-info__image"
                            src="https://vancouverhumanesociety.bc.ca/wp-content/uploads/2019/01/Upcoming-eventsiStock-978975308-e1564610924151-1024x627.jpg"
                            alt={eventInfo.name}/>
                    </section>
                </Paper>
            )
    );
};

export default EventInfo;