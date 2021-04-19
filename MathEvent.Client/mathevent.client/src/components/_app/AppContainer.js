import React from "react";
import { Provider } from "react-redux";
import { ConnectedRouter } from "connected-react-router";
import { store, history } from "../../store";
import App from "./App";
import AppHeader from "./AppHeader";
import "./App.scss";

const AppContainer = () => {
    return(
        <Provider store={store}>
            <ConnectedRouter history={history}>
                <AppHeader/>
                <App/>
            </ConnectedRouter>
        </Provider>
    );
};

export { history };
export default AppContainer;