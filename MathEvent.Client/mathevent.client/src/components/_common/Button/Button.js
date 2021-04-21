import React from "react";
import MuiButton from "@material-ui/core/Button";
import { Icon } from "../Icon";
import buttonTypes from "../../../constants/button-types";
import colors from "../../../constants/colors";
import "./Button.scss";

const Button = ({ className, type, color, startIcon, endIcon, onClick, children }) => {
    const buttonType = type ?? buttonTypes.contained;
    const buttonColor = color ?? colors.primary;

    return (
        <MuiButton
            className={`${className} button`}
            variant={buttonType}
            color={buttonColor}
            onClick={onClick}
            startIcon={startIcon ? <Icon type={startIcon}/> : null}
            endIcon={endIcon ? <Icon type={endIcon}/> : null}
        >
            {children}
        </MuiButton>
    );
};

export default Button;