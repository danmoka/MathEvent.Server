import React, { useEffect, useState } from "react";
import MuiCheckbox from "@material-ui/core/Checkbox";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import colors from "../../../constants/colors";

const Checkbox = ({ className, value, label, onChange }) => {
    const [checkboxValue, setCheckboxValue] = useState(value);

    useEffect(() => {
        setCheckboxValue(value);
    }, [value]);

    const handleChange = () => {
        const newValue = !checkboxValue;

        setCheckboxValue(newValue);
        onChange(newValue);
    };

    return (
        <FormControlLabel
            className={className}
            label={label}
            control={
                <MuiCheckbox
                    color={colors.primary}
                    checked={checkboxValue}
                    onChange={handleChange}
                    onMouseDown={(e) => e.stopPropagation()}
                />
            }
        />
    );
};

export default Checkbox;