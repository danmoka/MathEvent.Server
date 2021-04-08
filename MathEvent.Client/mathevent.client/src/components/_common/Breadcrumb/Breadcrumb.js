import React from 'react';
import Link from '@material-ui/core/Link';
import Typography from '@material-ui/core/Typography';

const Breadcrumb = ({ crumb, isLast, onClick }) => {
  return (
    isLast
      ? (<Typography>{crumb.name}</Typography>)
      : (
          <Link onClick={onClick}>
              {crumb.name}
          </Link>
        )
  );
};

export default Breadcrumb;