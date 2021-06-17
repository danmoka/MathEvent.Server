import React from "react";
import { Redirect as RouterRedirect } from "react-router";
import routes from "../../../utils/routes";

const Redirect = ({ to }) => <RouterRedirect to={to}/>;

export { routes };
export default Redirect;