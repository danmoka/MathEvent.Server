import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents } from "../../store/actions/event";
import AppContent from "./AppContent";
import AppSidebar from "./AppSidebar";

const App = () => {
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(fetchEvents());
    }, [dispatch]);

    return (
        <div className="app">
            <AppSidebar/>
            <div className="app__main">
                <AppContent/>
            </div>
        </div>
    );
};

export default App;