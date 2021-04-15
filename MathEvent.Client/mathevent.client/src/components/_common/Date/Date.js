import React from "react";
import Moment from "react-moment";

const Date = ({primaryText, date}) => {
    return (
        <h5>
            {primaryText}
            <Moment
            className="date__time"
            format=" MMMM Do YYYY, h:mm:ss a"
            date={date}
            withTitle
            locale="ru"/>
        </h5>
    );
};

export default Date;