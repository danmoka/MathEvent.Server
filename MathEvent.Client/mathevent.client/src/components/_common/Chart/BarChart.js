import React, { useEffect, useState } from 'react';
import { useSelector, useDispatch } from "react-redux";
import Paper from '@material-ui/core/Paper';
import {
    ArgumentAxis,
    BarSeries,
    Chart,
    Title,
    ValueAxis
} from '@devexpress/dx-react-chart-material-ui';
import { Palette } from '@devexpress/dx-react-chart';
import schemeCollection from "../../../constants/chart-color-scheme";

const BarChart = ({ data, valueField, argumentField, title, elevation=1 }) => {
    const dispatch = useDispatch();
    const { isDarkTheme } = useSelector(state => state.app);
    const [scheme, setScheme] = useState(schemeCollection.schemeCategory10);

    useEffect(() => {
        if (isDarkTheme) {
            setScheme(schemeCollection.schemeDark2);
        }
        else {
            setScheme(schemeCollection.schemeCategory10);
        }
    }, [dispatch, isDarkTheme]);

    return (
        <Paper
            elevation={elevation}>
            <Chart
                data={data}
            >
                <Palette scheme={scheme} />
                <ArgumentAxis />
                <ValueAxis/>
                <BarSeries
                    valueField={valueField}
                    argumentField={argumentField}
                />
                <Title
                    text={title}
                />
            </Chart>
        </Paper>
    );
};

export default BarChart;