import React from "react";
import { useSelector } from "react-redux";
import { Route } from "react-router-dom";
import Paper from "@material-ui/core/Paper";
import {
    navigateToEventsStatistics
} from "../../utils/navigator";
import routes from "../../utils/routes";
import TabPanel from "../_common/TabPanel";
import "./Statistics.scss";
import EventStatisticsPanel from "./event/EventStatisticsPanel";

const tabRoutes = [
    routes.statistics.event
];

const tabs = [
    { label: "События", onClick: () => navigateToEventsStatistics() }
];

const Statistics = () => {
    const currentRoute = useSelector((state) => state.router.location.pathname);

    return (
        <div className="statistics">
            <div className="statistics__tabs">
                <TabPanel tabs={tabs} value={tabRoutes.indexOf(currentRoute)}/>
            </div>
            <div className="statistics__content">
            <Route
                path={routes.statistics.event}
                component={EventStatisticsPanel}
            />
            </div>
        </div>
    );
};

export default Statistics;