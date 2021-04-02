import eventReducer from "./reducers/event";

const createRootReducer = () => ({
    event: eventReducer
});

export default createRootReducer;