import React from "react";
import SubcribersPieChart from "./SubscribersPieChart";
import "./EventStatistics.scss";
import BusiestMonthsBar from "./BusiestMonthsBar";

const EventStatisticsPanel = () => {
    return (
        <div className="event-statistics-panel">
            <div className="event-statistics-panel__item">
                <SubcribersPieChart/>
            </div>
            <div className="event-statistics-panel__item">
                <BusiestMonthsBar/>
            </div>
            <div className="event-statistics-panel__item">
                <SubcribersPieChart/>
            </div>
        </div>
    );
};

export default EventStatisticsPanel;