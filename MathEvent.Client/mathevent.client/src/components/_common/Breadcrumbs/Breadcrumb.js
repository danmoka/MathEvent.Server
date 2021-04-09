import React from 'react';
import Link from '@material-ui/core/Link';
import Typography from '@material-ui/core/Typography';

const Breadcrumb = ({ primaryText, index, isLast, onClick }) => {
  return (
    isLast
      ? (<Typography>{primaryText}</Typography>)
      : (
          <Link onClick={onClick}>
              {primaryText}
          </Link>
        )
  );
};

export default Breadcrumb;