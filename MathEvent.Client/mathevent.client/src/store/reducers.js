import { connectRouter } from "connected-react-router";
import accountReducer from "./reducers/account";
import appReducer from "./reducers/app";
import eventReducer from "./reducers/event";
import fileReducer from "./reducers/file";
import modalReducer from "./reducers/modal";
import organizationReducer from "./reducers/organization";

const createRootReducer = (history) => ({
    account: accountReducer,
    app: appReducer,
    event: eventReducer,
    file: fileReducer,
    modal: modalReducer,
    organization: organizationReducer,
    router: connectRouter(history),
});

export default createRootReducer;