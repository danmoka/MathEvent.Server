import React, { useEffect, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import Paper from '@material-ui/core/Paper';
import {
  Chart,
  PieSeries,
  Legend,
  Title,
} from '@devexpress/dx-react-chart-material-ui';
import { Palette } from '@devexpress/dx-react-chart';
import Tooltip from '@material-ui/core/Tooltip';
import Typography from '@material-ui/core/Typography';
import { cropText } from '../../../utils/text';
import schemeCollection from '../../../constants/chart-color-scheme';
import './Chart.scss';

const labelLength = 20;

const Label = ({ ...restProps }) => {
  const { text, croppedText } = cropText(labelLength, restProps.text || '');

  return (
    <Tooltip title={text} placement="bottom">
      <Typography
        className="chart__label"
        noWrap
        variant="caption"
        color="textSecondary"
      >
        {croppedText || text}
      </Typography>
    </Tooltip>
  );
};

const PieChart = ({
  data,
  valueField,
  argumentField,
  title,
  elevation = 1,
}) => {
  const dispatch = useDispatch();
  const { isDarkTheme } = useSelector((state) => state.app);
  const [scheme, setScheme] = useState(schemeCollection.schemeCategory10);

  useEffect(() => {
    if (isDarkTheme) {
      setScheme(schemeCollection.schemeDark2);
    } else {
      setScheme(schemeCollection.schemeCategory10);
    }
  }, [dispatch, isDarkTheme]);

  return (
    <Paper elevation={elevation}>
      <Chart data={data}>
        <Palette scheme={scheme} />
        <PieSeries
          valueField={valueField}
          argumentField={argumentField}
          innerRadius={0.6}
        />
        <Legend labelComponent={Label} />
        <Title text={title} />
      </Chart>
    </Paper>
  );
};

export default PieChart;
