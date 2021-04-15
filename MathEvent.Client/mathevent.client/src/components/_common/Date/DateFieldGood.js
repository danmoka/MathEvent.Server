// import React, { useEffect, useState } from 'react';
// import { createMuiTheme, MuiThemeProvider } from "@material-ui/core/styles";
// import Grid from '@material-ui/core/Grid';
// import DateFnsUtils from '@date-io/date-fns';
// import { MuiPickersUtilsProvider, KeyboardTimePicker, KeyboardDatePicker } from '@material-ui/pickers';
// import ruLocale from "date-fns/locale/ru";
// import palette from "../../../styles/palette";

// const theme = createMuiTheme({ palette });

// const DateField = ({
//     classNameMainForm,
//     classNameDatePicker,
//     dateLabel="Дата",
//     timeLabel="Время",
//     okLabel="Готово",
//     cancelLabel="Отмена",
//     value,
//     onChange,
//     onAccept,
//     minDate=Date.now(),
//     minDateMessage=`Выход за пределы минимальной даты: ${minDate}`,
//     }) => {
//     const [fieldValue, setFieldValue] = useState(value);

//     useEffect(() => {
//         setFieldValue(value);
//     }, [value]);

//     const handleChange = (value) => {
//         const newValue = value;

//         setFieldValue(newValue);
//         onChange(newValue);
//     };

//     const handleAccept = (value) => {
//         const acceptedValue = value

//         setFieldValue(acceptedValue);
//         onAccept(acceptedValue);
//     };

//     return (
//     <MuiThemeProvider theme={theme}>
//         <MuiPickersUtilsProvider
//             locale={ruLocale}
//             utils={DateFnsUtils}>
//             <Grid container className={classNameMainForm}>
//                 <KeyboardDatePicker
//                     className={classNameDatePicker}
//                     id="date-picker-dialog"
//                     label={dateLabel}
//                     okLabel={okLabel}
//                     cancelLabel={cancelLabel}
//                     format="MM/dd/yyyy"
//                     value={fieldValue}
//                     onChange={handleChange}
//                     onAccept={handleAccept}
//                     minDateMessage={minDateMessage}
//                     minDate={minDate}
//                 />
//                 <KeyboardTimePicker
//                     className={classNameDatePicker}
//                     id="time-picker"
//                     label={timeLabel}
//                     okLabel={okLabel}
//                     cancelLabel={cancelLabel}
//                     value={fieldValue}
//                     onChange={handleChange}
//                     onAccept={handleAccept}
//                     minDateMessage={minDateMessage}
//                     minDate={minDate}
//                 />
//             </Grid>
//         </MuiPickersUtilsProvider>
//     </MuiThemeProvider>
//     );
// };

// export default DateField;