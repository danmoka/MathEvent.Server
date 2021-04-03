import React from "react";
import { Provider } from "react-redux";
import { ConnectedRouter } from "connected-react-router";
import App from "./App";
import { store, history } from "../../store";
import "./App.scss";

const AppContainer = () => {
    return(
        <Provider store={store}>
            <ConnectedRouter history={history}>
                <App/>
            </ConnectedRouter>
        </Provider>
    );
};

export { history };
export default AppContainer;