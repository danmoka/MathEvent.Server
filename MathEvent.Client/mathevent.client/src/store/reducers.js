import { connectRouter } from "connected-react-router";
import eventReducer from "./reducers/event";

const createRootReducer = (history) => ({
    router: connectRouter(history),
    event: eventReducer
});

export default createRootReducer;