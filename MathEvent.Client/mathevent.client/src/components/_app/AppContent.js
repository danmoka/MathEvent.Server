import React from "react";
import { Route, Switch } from "react-router-dom";
import Events from "../events";
import Login from "../account";
import routes from "../../utils/routes";

const AppContent = () => (
    <div className="app__content">
        <Switch>
            <Route path={routes.events} component={Events}/>
            <Route path={routes.login} component={Login}/>
        </Switch>
    </div>
);

export default AppContent;