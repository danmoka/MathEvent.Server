import React from "react";
import { createMuiTheme, MuiThemeProvider } from "@material-ui/core";
import CircularProgress from "@material-ui/core/CircularProgress";
import palette from "../../../styles/palette";
import "./Loader.scss";

const theme = createMuiTheme({ palette });

const Loader = ({ className, color = "primary", size="medium" }) => (
    <MuiThemeProvider theme={theme}>
        <CircularProgress className={`${className} loader--${size}`} color={color}/>
    </MuiThemeProvider>
);

export default Loader;