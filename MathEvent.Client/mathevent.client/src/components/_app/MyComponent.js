import React from "react";
import { useSelector } from "react-redux";

const MyComponent = () => {
    const { events } = useSelector(state => state.event)
    return (
        events.map(event => <a>event.name</a>)
    )
}

export default MyComponent;