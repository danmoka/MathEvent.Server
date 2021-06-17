import React, { useEffect, useState } from "react";
import FormControl from "@material-ui/core/FormControl";
import InputLabel from "@material-ui/core/InputLabel";
import MenuItem from "@material-ui/core/MenuItem";
import Select from "@material-ui/core/Select";

const Dropdown = ({ className, items, label, value, onChange }) => {
    if (!items) {
        return null;
    }

    const [dropdownValue, setDropdownValue] = useState(value);

    useEffect(() => {
        setDropdownValue(value);
    }, [value]);

    const handleChange = (event) => {
        const newValue = event.target.value;

        setDropdownValue(newValue);
        onChange(newValue);
    };

    return (
        <FormControl className={className} variant="outlined">
            <InputLabel>{label}</InputLabel>
            <Select label={label} value={dropdownValue} variant="outlined" size="small" onChange={handleChange}>
                {items.map((item) => (
                <MenuItem key={`dropdown-item-${item.value}`} value={item.value}>
                    {item.name}
                </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
};

export default Dropdown;