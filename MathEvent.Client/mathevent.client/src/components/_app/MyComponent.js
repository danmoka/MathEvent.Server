import React from "react";
import { useSelector } from "react-redux";

const MyComponent = () => {
    const { events } = useSelector(state => state.event)
    return (
        events.map((event) => <p key={event.id}>{event.name} {event.startDate}\\\</p>)
    )
}

export default MyComponent;