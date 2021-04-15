import React, { Fragment, useEffect, useState } from 'react';
import { createMuiTheme, MuiThemeProvider } from "@material-ui/core";
import { DateTimePicker, MuiPickersUtilsProvider } from "@material-ui/pickers";
import DateFnsUtils from '@date-io/date-fns';
import ruLocale from "date-fns/locale/ru";
import palette from "../../../styles/palette";

const theme = createMuiTheme({ palette });

const DateField = ({
    className,
    label,
    value,
    onChange,
    minDate=Date.now(),
    minDateMessage=`Выход за пределы минимальной даты: ${minDate}`,
    }) => {
    const [fieldValue, setFieldValue] = useState(value);

    useEffect(() => {
        setFieldValue(value);
    }, [value]);

    const handleChange = (value) => {
        const newValue = value;

        setFieldValue(newValue);
        onChange(newValue);
    };

    return (
        <MuiThemeProvider theme={theme}>
            <MuiPickersUtilsProvider
                locale={ruLocale}
                utils={DateFnsUtils}>
                <DateTimePicker
                    className={className}
                    autoOk
                    ampm={false}
                    variant="inline"
                    value={fieldValue}
                    onChange={handleChange}
                    label={label}
                    minDateMessage={minDateMessage}
                    minDate={minDate}
                />
            </MuiPickersUtilsProvider>
        </MuiThemeProvider>
    );
};

export default DateField;