import React from 'react';
import Card from '@material-ui/core/Card';
import CardActionArea from '@material-ui/core/CardActionArea';
import CardContent from '@material-ui/core/CardContent';
import CardMedia from '@material-ui/core/CardMedia';
import Typography from '@material-ui/core/Typography';
import "./Grid.scss";

const GridCard = ({ primaryText, secondaryText, additionalInfo, isSelected, index, actions, onClick }) => {
  const classes = isSelected ? "grid-card grid-card--selected" : "grid-card";

  return (
    <Card
      className={classes}
      onClick={onClick}>
      <CardActionArea className="grid-card__action_area">
        <CardMedia
          className="grid-card__media"
          image="https://mimievent.ru/wp-content/uploads/2017/09/event-711x400.jpg"
          title={primaryText}
        />
        <CardContent>
          <Typography variant="h5" component="h2">
            {primaryText}
          </Typography>
          <Typography color="textSecondary">
            {secondaryText}
          </Typography>
          <Typography variant="body2" component="p">
            {additionalInfo}
          </Typography>
        </CardContent>
      </CardActionArea>
    </Card>
  );
};

export default GridCard;