import React from "react";
import { Provider } from "react-redux";
import { ConnectedRouter } from "connected-react-router";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { store, history } from "../../store";
import App from "./App";
import AppHeader from "./AppHeader";
import "./App.scss";
import palette from "../../styles/palette";

const theme = createMuiTheme({palette});

const AppContainer = () => {
    return(
        <Provider store={store}>
            <ConnectedRouter history={history}>
                <ThemeProvider theme={theme}>
                  <AppHeader/>
                  <App/>
                </ThemeProvider>
            </ConnectedRouter>
        </Provider>
    );
};

export { history };
export default AppContainer;