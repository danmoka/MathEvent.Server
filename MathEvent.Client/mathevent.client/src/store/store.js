import { configureStore } from "@reduxjs/toolkit";
import { routerMiddleware } from "connected-react-router";
import createRootReducer from "./reducers"

const rootReducer = createRootReducer();

const store = configureStore({
    reducer: rootReducer,
    middleware: getDefaultMiddleware =>
        getDefaultMiddleware({ serializableCheck: false })
            .concat(routerMiddleware())
});

export { store };