import { getRoute } from "../../utils/get-route";

const eventRoutes = {
    fetchEvents: (event=null) => event
        ? getRoute(`events/?parent=${event.id}`)
        : getRoute(`events/?parent=${null}`)
};

export default eventRoutes;