import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents } from "../../store/actions/event";
import { fetchUserInfo } from "../../store/actions/account";
import AppContent from "./AppContent";
import AppSidebar from "./AppSidebar";

const App = () => {
    const dispatch = useDispatch();
    const { hasToken } = useSelector(state => state.account);

    useEffect(() => {
        dispatch(fetchEvents());
    }, [dispatch]);

    useEffect(() => {
        dispatch(fetchUserInfo());
    }, [dispatch, hasToken]);

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