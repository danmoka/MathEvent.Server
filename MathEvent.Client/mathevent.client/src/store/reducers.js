import { connectRouter } from "connected-react-router";
import eventReducer from "./reducers/event";
import organizationReducer from "./reducers/organization";

const createRootReducer = (history) => ({
    router: connectRouter(history),
    event: eventReducer,
    organization: organizationReducer
});

export default createRootReducer;