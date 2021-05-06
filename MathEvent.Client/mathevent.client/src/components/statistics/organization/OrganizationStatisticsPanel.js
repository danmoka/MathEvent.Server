import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchStatistics } from "../../../store/actions/organization";
import PieChart from "../../_common/Chart/PieChart";
import BarChart from "../../_common/Chart/BarChart";
import "./OrganizationStatistics.scss";

const OrganizationStatisticsPanel = () => {
    const dispatch = useDispatch();
    const { statistics } = useSelector((state) => state.organization);

    useEffect(() => {
        dispatch(fetchStatistics());
    }, [dispatch]);

    return (
        <div className="organization-statistics-panel">
            {statistics.map((chart, index) => {
                switch(chart.type) {
                    case "pie":
                        return (
                            <div key={index} className="organization-statistics-panel__item">
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
                            <div key={index} className="organization-statistics-panel__item">
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
                            <div key={index} className="organization-statistics-panel__item">
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
    );
};

export default OrganizationStatisticsPanel;