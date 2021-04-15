import React, { useCallback, useState } from 'react';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import CardHeader from '@material-ui/core/CardHeader';
import CardMedia from '@material-ui/core/CardMedia';
import MuiListItemIcon from "@material-ui/core/ListItemIcon";
import MuiMenuItem from "@material-ui/core/MenuItem";
import Popover from "@material-ui/core/Popover";
import Typography from '@material-ui/core/Typography';
import { Icon, IconButton, iconTypes } from "../Icon";
import "./Grid.scss";
import "../List/List.scss";

const GridCard = ({ primaryText, secondaryText, additionalInfo, isSelected, index, actions, onClick }) => {
  const classes = isSelected ? "grid-card grid-card--selected" : "grid-card";
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
      className={classes}
      onClick={onClick}>
      <CardHeader
        title={primaryText}
        subheader={secondaryText}
        action={
          <>
            <IconButton
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
        }
      />
      <CardMedia
        className="grid-card__media"
        image="https://vancouverhumanesociety.bc.ca/wp-content/uploads/2019/01/Upcoming-eventsiStock-978975308-e1564610924151-1024x627.jpg"
        title={primaryText}
      />
      <CardContent>
        <Typography variant="body2" component="p">
          {additionalInfo}
        </Typography>
      </CardContent>
    </Card>
  );
};

export default GridCard;