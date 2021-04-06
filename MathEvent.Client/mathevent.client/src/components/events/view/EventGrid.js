import React from "react";
import { useSelector } from "react-redux";
import { Grid } from "@material-ui/core";
import EventGridCard from "./EventGridCard";
import EventHeader from "./EventHeader";
import Loader from "../../_common/Loader";

const EventGrid = () => {
    const { events, selectedEvent, isFetching } = useSelector(state => state.event);

    return (
        <div className="event-grid">
            <EventHeader headerText="Карточки событий"/>
            {isFetching
                ? (<Loader className="event-grid__loader" size="medium"/>)
                : (
                    <Grid
                        className="event-grid__grid"
                        container 
                        spacing={2}
                        direction="row"
                        justifyContent="flex-start"
                        alignItems="center">
                            {events.map((event, index) => (
                                <Grid key={event.id} item>
                                    <EventGridCard
                                        key={event.id}
                                        event={event}
                                        isSelected={selectedEvent && event.id === selectedEvent.id}
                                        index={index + 1}
                                    />
                                </Grid>
                            ))}
                    </Grid>
                )}
        </div>
    );
};

export default EventGrid;