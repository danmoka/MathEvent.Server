import React from "react";
import Moment from "react-moment";
import Typography from "@material-ui/core/Typography";

const Date = ({primaryText, date}) => {
    return (
        <Typography variant="h6" gutterBottom>
            {primaryText}
            <Moment
            className="date__time"
            format=" MMMM Do YYYY, h:mm:ss a"
            date={date}
            withTitle
            locale="ru"/>
        </Typography>
    );
};

export default Date;