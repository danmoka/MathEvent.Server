import React from "react";
import { Route, Switch } from "react-router-dom";
import Events from "../events";
import Home from "../home";
import Login from "../account";
import routes from "../../utils/routes";

const AppContent = () => (
    <div className="app__content">
        <Switch>
            <Route path="/" exact component={Home}/>
            <Route path={routes.home} component={Home}/>
            <Route path={routes.events} component={Events}/>
            <Route path={routes.login} component={Login}/>
        </Switch>
    </div>
);

export default AppContent;