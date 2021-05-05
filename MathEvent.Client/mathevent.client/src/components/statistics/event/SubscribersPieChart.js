import React from "react";
import PieChart from '../../_common/Chart/PieChart';

const SubcribersPieChart = () => {
    const data = [
        { event: 'Чаепитие', number: 12 },
        { event: 'Футбол', number: 7 },
        { event: 'Настольные игры', number: 7 },
        { event: 'Олимпиала Конфирмит', number: 7 },
        { event: 'Конференция по схемотехнике', number: 2 },
        { event: 'Веселые старты', number: 5 },
        { event: 'Вечера в общаге', number: 17 },
        { event: 'Остальные', number: 55 },
    ];
    const title="Самые популярные события"
    const valueField = "number";
    const argumentField = "event"; 

    return (
        <PieChart
            data={data}
            title={title}
            valueField={valueField}
            argumentField={argumentField}
        />
      );
};

export default SubcribersPieChart;