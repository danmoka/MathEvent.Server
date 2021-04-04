import React, { useCallback, useState } from "react";
import { makeStyles } from "@material-ui/core";
import MuiMenuItem from "@material-ui/core/MenuItem";
import MuiListItem from "@material-ui/core/ListItem";
import MuiListItemText from "@material-ui/core/ListItemText";
import MuiListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";
import MuiListItemIcon from "@material-ui/core/ListItemIcon";
import Popover from "@material-ui/core/Popover";
import { Icon, IconButton, iconTypes } from "../../_common/Icon";
import "./ListItem.scss";

const useStyles = makeStyles({
    primary: {
        fontSize: "18px"
    },
    secondary: {
        fontSize: "14px"
    }
});

const ListItem = ({ primaryText, secondaryText, isSelected, index, actions, onClick }) => {
    const [isHovered, setIsHovered] = useState(false);
    const [anchorEl, setAnchorEl] = useState(null);
    const classes = useStyles();

    const handleMenuOpen = (e) => {
        e.stopPropagation();
        setAnchorEl(e.currentTarget);
        setIsHovered(false);
    };

    const handleMenuClose = (e) => {
        e.stopPropagation();
        setAnchorEl(null);
    };

    const handleMouseEnter = useCallback(() => {
        setIsHovered(true);
    }, []);

    const handleMouseLeave = useCallback(() => {
        setIsHovered(false);
    }, []);

    return (
        <MuiListItem
            className="list-item"
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
            onClick={onClick}
            selected={isSelected}
            button
        >
            {/* <MuiListItemIcon>
                <Badge content={index} color={isSelected ? "primary" : "secondary"}/>
            </MuiListItemIcon> */}
            <MuiListItemText
                classes={classes}
                primary={primaryText}
                secondary={secondaryText}
            />

            {actions && (
                <>
                    <MuiListItemSecondaryAction
                        className={isHovered ? "list-item__secondary--hovered" : "list-item__secondary"}
                        onClick={handleMenuOpen}
                    >
                        <IconButton type={iconTypes.more}/>
                    </MuiListItemSecondaryAction>
                    <Popover
                        id="list-item-popover"
                        open={Boolean(anchorEl)}
                        anchorEl={anchorEl}
                        onClose={handleMenuClose}
                        anchorOrigin={{ vertical: "top", horizontal: "right", }}
                        transformOrigin={{ vertical: "top", horizontal: "right", }}
                    >
                        <div className="list-item__secondary-menu">
                            {actions.map((action) => (
                                <MuiMenuItem key={action.id} onClick={(e) => action.onClick(e, handleMenuClose)}>
                                    <MuiListItemIcon>
                                        <Icon type={action.icon}/>
                                    </MuiListItemIcon>
                                    {action.label}
                                </MuiMenuItem>
                            ))}
                        </div>
                    </Popover>
                </>
            )}
        </MuiListItem>
    );
};

export default ListItem;