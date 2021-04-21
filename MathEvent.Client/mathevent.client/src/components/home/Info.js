import React from "react";
import Typography from '@material-ui/core/Typography';
import colors from "../../constants/colors";

const Info = ({ title, text }) => {
    return (
        <div className="info">
            <Typography align="center">
                {title}
            </Typography>
            <hr/>
            <Typography
                color="textSecondary"
                align="center">
                    {text}
            </Typography>
        </div>
    );
};

export default Info;