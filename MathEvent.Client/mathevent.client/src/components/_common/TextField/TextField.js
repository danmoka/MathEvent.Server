import React, { useEffect, useState } from "react";
import MuiTextField from "@material-ui/core/TextField";
import colors from "../../../constants/colors";

const TextField = ({
  className,
  label,
  type,
  value,
  error,
  helperText,
  multiline,
  rows,
  onChange,
  onFocus,
  onFocusOut,
}) => {
  const [fieldValue, setFieldValue] = useState(value);

  useEffect(() => {
    setFieldValue(value);
  }, [value]);

  const handleChange = (event) => {
    const newValue = event.target.value;

    setFieldValue(newValue);
    onChange(newValue);
  };

  return (
    <MuiTextField
        className={className}
        color={colors.primary}
        error={error}
        helperText={helperText}
        label={label}
        size="small"
        type={type}
        value={fieldValue}
        variant="outlined"
        multiline={multiline}
        rows={multiline ? rows : null}
        onChange={handleChange}
        onFocus={onFocus}
        onBlur={onFocusOut}
        InputLabelProps={{
          shrink: true,
        }}
      />
  );
};

export default TextField;