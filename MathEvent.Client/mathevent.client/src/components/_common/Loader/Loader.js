import React from "react";
import CircularProgress from "@material-ui/core/CircularProgress";
import colors from "../../../constants/colors";
import "./Loader.scss";

const Loader = ({ className, color = colors.primary, size="medium" }) => (
    <CircularProgress className={`${className} loader--${size}`} color={color}/>
);

export default Loader;