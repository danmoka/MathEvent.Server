import React from 'react';
import Link from '@material-ui/core/Link';
import Tooltip from '@material-ui/core/Tooltip';
import Typography from '@material-ui/core/Typography';
import { cropText } from '../../../utils/text';

const textLength = 10;

const Breadcrumb = ({ primaryText, index, isLast, onClick }) => {
  const { text: originalPrimaryText, croppedText: croppedPrimaryText } =
    cropText(textLength, primaryText || '');

  return isLast ? (
    <Tooltip title={originalPrimaryText}>
      <Typography variant="body1">
        {croppedPrimaryText || originalPrimaryText}
      </Typography>
    </Tooltip>
  ) : (
    <Tooltip title={originalPrimaryText}>
      <Typography variant="body1">
        <Link onClick={onClick}>
          {croppedPrimaryText || originalPrimaryText}
        </Link>
      </Typography>
    </Tooltip>
  );
};

export default Breadcrumb;
