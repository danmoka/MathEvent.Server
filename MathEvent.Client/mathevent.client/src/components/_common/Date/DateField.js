import React, { useEffect, useState } from 'react';
import { DateTimePicker, MuiPickersUtilsProvider } from "@material-ui/pickers";
import DateFnsUtils from '@date-io/date-fns';
import ruLocale from "date-fns/locale/ru";

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
    );
};

export default DateField;