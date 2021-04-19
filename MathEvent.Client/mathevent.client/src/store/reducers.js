import { connectRouter } from "connected-react-router";
import accountReducer from "./reducers/account";
import eventReducer from "./reducers/event";
import fileReducer from "./reducers/file";
import organizationReducer from "./reducers/organization";

const createRootReducer = (history) => ({
    router: connectRouter(history),
    account: accountReducer,
    event: eventReducer,
    organization: organizationReducer,
    file: fileReducer
});

export default createRootReducer;