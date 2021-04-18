import { connectRouter } from "connected-react-router";
import eventReducer from "./reducers/event";
import organizationReducer from "./reducers/organization";
import fileReducer from "./reducers/file";

const createRootReducer = (history) => ({
    router: connectRouter(history),
    event: eventReducer,
    organization: organizationReducer,
    file: fileReducer
});

export default createRootReducer;