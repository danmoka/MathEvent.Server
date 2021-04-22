import React, { useCallback, useState } from "react";
import Card from '@material-ui/core/Card';
import MuiListItemIcon from "@material-ui/core/ListItemIcon";
import MuiMenuItem from "@material-ui/core/MenuItem";
import Popover from "@material-ui/core/Popover";
import Typography from '@material-ui/core/Typography';
import { Icon, IconButton, iconTypes } from "../Icon";
import FileImage from "./FileImage";
import "./File.scss";
import "../List/List.scss";

const File = ({ name, ext, hierarchy, actions, onClick}) => {
    const [anchorEl, setAnchorEl] = useState(null);

    const handleMenuOpen = (e) => {
        e.stopPropagation();
        setAnchorEl(e.currentTarget);
    };

    const handleMenuClose = (e) => {
        e.stopPropagation();
        setAnchorEl(null);
    };

    const handleSecondaryAction = useCallback((e, action) => {
        handleMenuClose(e);
        action();
        }, [handleMenuClose]);

    return (
      <Card
        className="file"
        onClick={onClick}>
          <FileImage
              className="file__media"
              ext={ext}
              hierarchy={hierarchy}
              alt={name}
          />
          <div className="file__content">
            <Typography className="file__name" variant="subtitle1" gutterBottom>
                {name}
            </Typography>
            <>
              <IconButton
                size="small"
                type={iconTypes.more}
                onClick={handleMenuOpen}
              />
              <Popover
                id="list-item-popover"
                open={Boolean(anchorEl)}
                anchorEl={anchorEl}
                onClose={handleMenuClose}
                anchorOrigin={{ vertical: "top", horizontal: "right" }}
                transformOrigin={{ vertical: "top", horizontal: "right" }}
              >
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
          </div>
      </Card>
    );
};

export default File;