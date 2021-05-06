const routes = {
    home: "/home",
    account: {
        login: "/login",
        register: "/register",
    },
    events: {
        main: "/events",
        edit: (eventId) => `events/${eventId}/edit`,
    },
    organizations: {
        main: "/organizations",
    },
    statistics: {
        main: "/statistics",
        event: "/statistics/event",
        organization: "/statistics/organization"
    }
};

export default routes;