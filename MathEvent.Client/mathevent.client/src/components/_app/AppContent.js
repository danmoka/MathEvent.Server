import React from "react";
import { Route, Switch } from "react-router-dom";
import Events from "../events";
import Home from "../home";
import Login from "../account/Login";
import Register from "../account/Register";
import ModalRoot from "../_common/Modal/ModalRoot";
import routes from "../../utils/routes";

const AppContent = () => (
    <div>
        <Switch>
            <Route path="/" exact component={Home}/>
            <Route path={routes.home} component={Home}/>
            <Route path={routes.events} component={Events}/>
            <Route path={routes.login} component={Login}/>
            <Route path={routes.register} component={Register}/>
        </Switch>
        <ModalRoot/>
    </div>
);

export default AppContent;