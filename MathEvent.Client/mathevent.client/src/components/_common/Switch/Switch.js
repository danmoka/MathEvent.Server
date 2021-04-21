import React, { useState } from "react";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import MuiSwitch from "@material-ui/core/Switch";
import colors from "../../../constants/colors";

const Switch = ({ checked, label, onChange }) => {
    const [isChecked, setIsChecked] = useState(checked);

    const handleChange = (e) => {
        const value = e.target.checked;
        setIsChecked(value);
        onChange(value);
    };

    return (
        <FormControlLabel
            control={
                <MuiSwitch
                    checked={isChecked}
                    color={colors.primary}
                    onChange={handleChange}
                />
            }
            label={label}
        />
    );
};

export default Switch;