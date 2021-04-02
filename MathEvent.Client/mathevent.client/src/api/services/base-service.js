const baseService = {
    get: async (url) => {
        try {
            return await fetch(url, {
                method: "GET"
            });
        } catch (e) {
            console.log(e);
        }
    },
}

export default baseService;