import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents } from "../../store/actions/event";
import MyComponent from "./MyComponent";

const App = () => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchEvents());
    }, [dispatch]);

    return (
        <div className="app">
            <MyComponent/>
        </div>
    )
}

export default App;