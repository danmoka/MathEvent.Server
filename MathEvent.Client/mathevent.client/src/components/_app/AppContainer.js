import React from "react";
import { Provider} from "react-redux";
import { ConnectedRouter } from "connected-react-router";
import { store, history } from "../../store";
import AppMain from "./AppMain";
import "./App.scss";

const AppContainer = () => {
    return(
        <Provider store={store}>
            <ConnectedRouter history={history}>
                  <AppMain/>
            </ConnectedRouter>
        </Provider>
    );
};

export { history };
export default AppContainer;