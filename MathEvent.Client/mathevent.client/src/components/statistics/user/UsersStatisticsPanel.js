import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchStatistics } from "../../../store/actions/user";
import BarChart from "../../_common/Chart/BarChart";
import Loader from "../../_common/Loader";
import PieChart from "../../_common/Chart/PieChart";
import "./UsersStatistics.scss";

const UsersStatisticsPanel = () => {
    const dispatch = useDispatch();
    const { statistics, isFetchingUserStatistics } = useSelector((state) => state.user);
    const [activeUsersTop, setActiveUsersTop] = useState(20);

    useEffect(() => {
        dispatch(fetchStatistics(activeUsersTop));
    }, [dispatch, activeUsersTop]);

    return (
        isFetchingUserStatistics || statistics.length < 1
        ? (<Loader className="users-statistics-panel__loader" size="medium"/>)
        : (
            <div className="users-statistics-panel">
                {statistics.map((chart, index) => {
                    switch(chart.type) {
                        case "pie":
                            return (
                                <div key={index} className="users-statistics-panel__item">
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
                                <div key={index} className="users-statistics-panel__item">
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
                                <div key={index} className="users-statistics-panel__item">
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

export default UsersStatisticsPanel;