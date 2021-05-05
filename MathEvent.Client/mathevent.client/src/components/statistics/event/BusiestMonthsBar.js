import React from "react";
import BarChart from '../../_common/Chart/BarChart';

const BusiestMonthsBar = () => {
    const data = [
        { mounth: 'январь', count: 11 },
        { mounth: 'февраль', count: 7 },
        { mounth: 'март', count: 5 },
        { mounth: 'апрель', count: 5 },
        { mounth: 'май', count: 8 },
        { mounth: 'июнь', count: 9 },
        { mounth: 'июль', count: 2 },
    ];
    const title="Самые занятые месяца за последний год"
    const valueField = "count";
    const argumentField = "mounth"; 

    return (
        <BarChart
            data={data}
            title={title}
            valueField={valueField}
            argumentField={argumentField}
        />
      );
};

export default BusiestMonthsBar;