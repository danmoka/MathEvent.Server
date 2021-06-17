import React from "react";
import { IconButton as MaterialIconButton } from "@material-ui/core";
import colors from "../../../constants/colors";
import icons from "./icons";
import "./Icon.scss";

const Icon = ({ type }) => {
    const SpecificIcon = icons[type];

    return <SpecificIcon />;
};

const IconButton = ({ className, color, type, size, selected, onClick }) => {
    const SpecificIcon = icons[type];
    color = color ? color : "default";
    color = selected ? colors.primary : color;
    const classes = className ? className : "icon";
    const sizes = size ? size : "medium";

    return (
        <MaterialIconButton className={classes} onClick={onClick} size={sizes} color={color}>
            <SpecificIcon style={{ color: color }} />
        </MaterialIconButton>
    );
};

export { Icon, IconButton };