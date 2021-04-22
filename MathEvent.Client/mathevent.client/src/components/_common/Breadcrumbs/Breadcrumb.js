import React from 'react';
import Link from '@material-ui/core/Link';
import Typography from '@material-ui/core/Typography';

const Breadcrumb = ({ primaryText, index, isLast, onClick }) => {
  return (
    isLast
      ? (<Typography variant="body1">{primaryText}</Typography>)
      : (
        <Typography variant="body1">
          <Link onClick={onClick}>
            {primaryText}
          </Link>
        </Typography>
        )
  );
};

export default Breadcrumb;