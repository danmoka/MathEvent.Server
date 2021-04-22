import React from "react";
import { useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import { Date } from "../../_common/Date";
import EventFiles from "./EventFiles";
import Image from "../../_common/Image";
import Loader from "../../_common/Loader";
import images from "../../../constants/images";

const EventInfo = () => {
    const { eventInfo, isFetchingEvent } = useSelector(state => state.event);
    const { isDarkTheme } = useSelector(state => state.app);

    if (!eventInfo) {
        return null;
    }

    const image = isDarkTheme ? images.eventDefaultDark : images.eventDefault;

    return (
        isFetchingEvent
            ? (<Loader className="event-grid__loader" size="medium"/>)
            : (
                <Paper className="event-info">
                    <section className="event-info__info">
                        <section className="event-info__info--image">
                            <Image
                                className="event-info__image"
                                src={image}
                                alt={eventInfo.name}/>
                        </section>
                        <section className="event-info__info--desctirption">
                            <Typography variant="h4" gutterBottom>{eventInfo.name}</Typography>
                            <Date
                                primaryText="Дата начала:"
                                date={eventInfo.startDate}/>
                            <Typography variant="body1" gutterBottom>{eventInfo.description}</Typography>
                            <Typography variant="body1" gutterBottom>{`Организация: ${eventInfo.organization ? eventInfo.organization.name : "отсутствует"}`}</Typography>
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