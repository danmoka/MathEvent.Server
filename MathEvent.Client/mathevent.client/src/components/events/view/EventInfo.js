import React from "react";
import Paper from "@material-ui/core/Paper";

const EventInfo = ({ event }) => {
    if (!event) {
        return null;
    }

    return (
        <Paper className="event-info">
            <section className="event-info__section--description">
                <h4>{event.name}</h4>
                <hr/>
                <h5>{`Дата начала: ${event.startDate}`}</h5>
                <h5>{`Описание: ${event.description}`}</h5>
                <h6>{`Организация: ${event.organization ? event.organization.name : "отсутствует"}`}
                </h6>
            </section>
            <section className="event-info__section--image">
                <img
                    className="event-info__image"
                    src="https://mimievent.ru/wp-content/uploads/2017/09/event-711x400.jpg"
                    alt={event.name}/>
            </section>
        </Paper>
    );
};

export default EventInfo;