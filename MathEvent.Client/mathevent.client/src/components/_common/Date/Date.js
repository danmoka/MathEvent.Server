import React from "react";
import Moment from "react-moment";
import Typography from "@material-ui/core/Typography";

const Date = ({primaryText, date, variant="h6"}) => {
    return (
        <Typography variant={variant} gutterBottom>
            {primaryText}
            <Moment
            className="date__time"
            format="LLLL"
            date={date}
            withTitle
            locale="ru"/>
        </Typography>
    );
};

export default Date;