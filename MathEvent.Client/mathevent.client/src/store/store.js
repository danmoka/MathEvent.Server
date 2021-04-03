import { configureStore } from "@reduxjs/toolkit";
import { createBrowserHistory } from "history";
import { routerMiddleware } from "connected-react-router";
import createRootReducer from "./reducers"

const history = createBrowserHistory();
const rootReducer = createRootReducer(history);

const store = configureStore({
    reducer: rootReducer,
    middleware: getDefaultMiddleware =>
        getDefaultMiddleware({ serializableCheck: false })
            .concat(routerMiddleware(history))
});

export { store, history };