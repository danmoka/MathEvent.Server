import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchStatistics } from "../../../store/actions/event";
import BarChart from "../../_common/Chart/BarChart";
import Loader from "../../_common/Loader";
import PieChart from "../../_common/Chart/PieChart";
import "./EventStatistics.scss";

const EventStatisticsPanel = () => {
    const dispatch = useDispatch();
    const { statistics, isFetchingEventStatistics } = useSelector((state) => state.event);
    const [eventSubsStatisticsTop, setEventSubsStatisticsTop] = useState(10);

    useEffect(() => {
        dispatch(fetchStatistics(eventSubsStatisticsTop));
    }, [dispatch, eventSubsStatisticsTop]);

    return (
        isFetchingEventStatistics || statistics.length < 1
        ? (<Loader className="event-statistics-panel__loader" size="medium"/>)
        : (
            <div className="event-statistics-panel">
                {statistics.map((chart, index) => {
                    switch(chart.type) {
                        case "pie":
                            return (
                                <div key={index} className="event-statistics-panel__item">
                                    <PieChart
                                        data={chart.data}
                                        title={chart.title}
                                        valueField={chart.valueField}
                                        argumentField={chart.argumentField}
                                    />
                                </div>
                            );
                        case "bar":
                            return (
                                <div key={index} className="event-statistics-panel__item">
                                    <BarChart
                                        data={chart.data}
                                        title={chart.title}
                                        valueField={chart.valueField}
                                        argumentField={chart.argumentField}
                                    />
                                </div>
                            );
                        default:
                            return (
                                <div key={index} className="event-statistics-panel__item">
                                    <PieChart
                                        data={chart.data}
                                        title={chart.title}
                                        valueField={chart.valueField}
                                        argumentField={chart.argumentField}
                                    />
                                </div>
                            );
                    };
                })}
            </div>
        )
    );
};

export default EventStatisticsPanel;