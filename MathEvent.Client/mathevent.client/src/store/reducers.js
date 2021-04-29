import { connectRouter } from "connected-react-router";
import accountReducer from "./reducers/account";
import appReducer from "./reducers/app";
import eventReducer from "./reducers/event";
import fileReducer from "./reducers/file";
import modalReducer from "./reducers/modal";
import organizationReducer from "./reducers/organization";
import userReducer from "./reducers/user";

const createRootReducer = (history) => ({
    account: accountReducer,
    app: appReducer,
    event: eventReducer,
    file: fileReducer,
    modal: modalReducer,
    organization: organizationReducer,
    router: connectRouter(history),
    user: userReducer
});

export default createRootReducer;