import { connectRouter } from "connected-react-router";
import accountReducer from "./reducers/account";
import appReducer from "./reducers/app";
import eventReducer from "./reducers/event";
import fileReducer from "./reducers/file";
import organizationReducer from "./reducers/organization";

const createRootReducer = (history) => ({
    account: accountReducer,
    app: appReducer,
    router: connectRouter(history),
    event: eventReducer,
    organization: organizationReducer,
    file: fileReducer
});

export default createRootReducer;