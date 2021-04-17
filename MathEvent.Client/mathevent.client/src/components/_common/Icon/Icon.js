import React from "react";
import { IconButton as MaterialIconButton } from "@material-ui/core";
import palette from "../../../styles/palette";
import icons from "./icons";
import "./Icon.scss";

const Icon = ({ type }) => {
    const SpecificIcon = icons[type];

    return <SpecificIcon />;
};

const IconButton = ({ type, selected, onClick }) => {
    const SpecificIcon = icons[type];
    const color = selected ? palette.primary.main : "";

    return (
        <MaterialIconButton className="icon" onClick={onClick}>
            <SpecificIcon style={{ color: color }} />
        </MaterialIconButton>
    );
};

export { Icon, IconButton };