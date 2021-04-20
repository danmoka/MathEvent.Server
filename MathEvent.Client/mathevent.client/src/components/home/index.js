import React from "react";
import Home from "./Home";
import { useTitle } from "../../hooks";
import "./Home.scss";

const HomePage = () => {
    useTitle("MathEvent");
    return <Home/>;
};

export default HomePage;