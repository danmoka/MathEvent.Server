import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents } from "../../store/actions/event";
import { fetchUserInfo } from "../../store/actions/account";
import AppContent from "./AppContent";
import AppSidebar from "./AppSidebar";

const App = () => {
    const dispatch = useDispatch();
    const { hasToken } = useSelector(state => state.account);
    const { header } = useSelector(state => state.app);

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
                <header className="app__page-header">
                    <div className="app__page-header-title">{header}</div>
                </header>
                <AppContent/>
            </div>
        </div>
    );
};

export default App;