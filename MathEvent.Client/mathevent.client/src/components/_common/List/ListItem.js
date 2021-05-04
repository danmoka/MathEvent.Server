import React, { useCallback, useState } from "react";
import { makeStyles, useTheme } from '@material-ui/core/styles';
import Avatar from '@material-ui/core/Avatar';
import MuiMenuItem from "@material-ui/core/MenuItem";
import MuiListItem from "@material-ui/core/ListItem";
import MuiListItemText from "@material-ui/core/ListItemText";
import MuiListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";
import MuiListItemIcon from "@material-ui/core/ListItemIcon";
import Popover from "@material-ui/core/Popover";
import { Icon, IconButton, iconTypes } from "../Icon";
import Checkbox from "../Checkbox";
import "./List.scss";

const useItemTextStyles = makeStyles({
    primary: {
        fontSize: "18px"
    },
    secondary: {
        fontSize: "14px"
    }
});

const useStyles = makeStyles((theme) => ({
    notSelected: {
        width: '100%',
        listStyleType: 'none',
        borderLeft: `6px !important`
    },
    selected: {
        width: '100%',
        listStyleType: 'none',
        borderLeft: `6px solid ${theme.palette.primary.main} !important`
    },
    avatar: {
        backgroundColor: theme.palette.primary.main,
        marginRight: 10
    }
  }));

const ListItem = ({ id, primaryText, secondaryText, avatarText, isSelected, checked, index, actions, onClick, onCheck }) => {
    const classes = useStyles(useTheme());
    const listItemTextClasses = useItemTextStyles();
    const [isHovered, setIsHovered] = useState(false);
    const [anchorEl, setAnchorEl] = useState(null);

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

    const handleSecondaryAction = useCallback((e, action) => {
        handleMenuClose(e);
        action();
      }, [handleMenuClose]);

    const handleCheck = useCallback((newValue) => {
        onCheck(id, newValue);
    }, [id, onCheck]);

    return (
        <MuiListItem
            className={isSelected ? classes.selected : classes.notSelected}
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
            onClick={onClick}
            selected={isSelected}
            button>

            {avatarText && (
                <Avatar aria-label="Avatar" className={classes.avatar}>
                    {avatarText}
                </Avatar>
            )}

            {onCheck && (
                <MuiListItemIcon>
                    <Checkbox value={checked} onChange={handleCheck} />
                </MuiListItemIcon>
            )}

            <MuiListItemText classes={listItemTextClasses} primary={primaryText} secondary={secondaryText}/>

            {actions && (
                <>
                    <MuiListItemSecondaryAction
                        className={isHovered ? "list-item__secondary--hovered" : "list-item__secondary"}
                        onClick={handleMenuOpen}>
                        <IconButton type={iconTypes.more}/>
                    </MuiListItemSecondaryAction>
                    <Popover
                        id="list-item-popover"
                        open={Boolean(anchorEl)}
                        anchorEl={anchorEl}
                        onClose={handleMenuClose}
                        anchorOrigin={{ vertical: "top", horizontal: "right" }}
                        transformOrigin={{ vertical: "top", horizontal: "right" }}>
                        <div className="list-item__secondary-menu">
                        {actions.map((action) => (
                            <MuiMenuItem key={action.id} onClick={(e) => handleSecondaryAction(e, action.onClick)}>
                                <MuiListItemIcon>
                                    <Icon type={action.icon} />
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