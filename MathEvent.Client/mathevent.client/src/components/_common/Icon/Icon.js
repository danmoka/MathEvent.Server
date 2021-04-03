import React from "react";
import { IconButton as MaterialIconButton } from "@material-ui/core";
import { OverlayTrigger, Tooltip } from "react-bootstrap";
import palette from "../../../styles/palette";
import icons from "./icons";
import "./Icon.scss";

const Icon = ({ type }) => {
    const SimpleIcon = icons[type];

    return <SimpleIcon/>
};

const IconButton = ({ type, selected, tooltip, tooltipPosition, tooltipWithMargin, onClick }) => {
    const Icon = <InnerIcon type={type} selected={selected} onClick={onClick}/>;

    if (tooltip) {
        return (
            <InnerTooltipIcon tooltip={tooltip} position={tooltipPosition} withMargin={tooltipWithMargin}>
                {Icon}
            </InnerTooltipIcon>
        );
    }

    return Icon;
};

const InnerIcon = ({ type, selected, onClick }) => {
    const Icon = icons[type];
    const color = selected ? palette.primary.main : "";

    return (
        <MaterialIconButton className="icon" onClick={onClick}>
            <Icon style={{ color: color }}/>
        </MaterialIconButton>
    );
};

const InnerTooltipIcon = ({ tooltip, position, withMargin, children }) => {
    const margins = { right: "Left", left: "Right", top: "Bottom", bottom: "Top"};

    return (
        <OverlayTrigger
            placement={position}
            overlay={
                <Tooltip
                    id={`tooltip-${tooltip}`}
                    style={withMargin ? { [`margin${margins[position]}`]: "8px" } : {}}>
                    {tooltip}
                </Tooltip>
            }
            >
                <div>{children}</div>
            </OverlayTrigger>
    );
};

export { Icon, IconButton };