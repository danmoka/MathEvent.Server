import React from 'react';
import { Grid } from '@material-ui/core';
import GridCard from './GridCard';

const CommonGrid = ({ items }) => (
  <Grid
    container
    spacing={2}
    direction="row"
    justify="flex-start"
    alignItems="flex-start"
  >
    {items.map((item) => (
      <Grid key={item.id} item>
        <GridCard
          key={item.id}
          primaryText={item.primaryText}
          secondaryText={item.secondaryText}
          additionalInfo={item.additionalInfo}
          image={item.image}
          isSelected={item.isSelected}
          index={item.index}
          onClick={item.onClick}
          actions={item.actions}
        />
      </Grid>
    ))}
  </Grid>
);

export default CommonGrid;
