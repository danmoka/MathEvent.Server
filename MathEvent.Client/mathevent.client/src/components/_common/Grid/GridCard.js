import React, { useCallback, useState } from 'react';
import { makeStyles, useTheme } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import CardHeader from '@material-ui/core/CardHeader';
import CardMedia from '@material-ui/core/CardMedia';
import MuiListItemIcon from "@material-ui/core/ListItemIcon";
import MuiMenuItem from "@material-ui/core/MenuItem";
import Popover from "@material-ui/core/Popover";
import Typography from '@material-ui/core/Typography';
import { Icon, IconButton, iconTypes } from "../Icon";
import Image from "../../_common/Image";
import "./Grid.scss";
import "../List/List.scss";

const width = 245;

const useStyles = makeStyles((theme) => ({
  notSelected: {
    width: width,
    borderBottom: `6px !important`
  },
  selected: {
    width: width,
    borderBottom: `6px solid ${theme.palette.primary.main} !important`
  },
}));

const GridCard = ({ primaryText, secondaryText, additionalInfo, image, isSelected, index, actions, onClick }) => {
  const classes = useStyles(useTheme());
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
      className={isSelected ? classes.selected : classes.notSelected}
      onClick={onClick}>
      <CardHeader
        titleTypographyProps={{variant:'body1' }}
        title={primaryText}
        subheaderTypographyProps={{variant:'body2' }}
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
        title={primaryText}>
          <div className="grid-card__box">
            <Image
              className="grid-card__image"
              src={image}
              alt={primaryText}/>
          </div>
      </CardMedia>
      <CardContent>
        <Typography variant="body2" color="textSecondary" component="p">
          {additionalInfo}
        </Typography>
      </CardContent>
    </Card>
  );
};

export default GridCard;