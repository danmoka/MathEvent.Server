const routes = {
    home: "/home",
    account: {
        login: "/login",
        register: "/register",
    },
    events: {
        main: "/events",
        edit: (eventId) => `/events/${eventId}/edit`,
    },
    organizations: {
        main: "/organizations",
    },
    statistics: {
        main: "/statistics",
        events: "/statistics/event",
        organizations: "/statistics/organization",
        users: "/statistics/user"
    }
};

export default routes;