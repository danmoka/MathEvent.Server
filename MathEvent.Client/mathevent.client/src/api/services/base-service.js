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
    put: async (url, data) => {
        try {
            return await fetch(url, {
                method: "PUT",
                body: JSON.stringify(data),
                headers: {
                    "Content-Type": "application/json"
                }
            });
        } catch (e) {
            console.log(e);
        }
    },
    patch: async (url, data) => {
        try {
            return await fetch(url, {
                method: "PATCH",
                body: JSON.stringify(data),
                headers: {
                    "Content-Type": "application/json"
                }
            });
        } catch (e) {
            console.log(e);
        }
    }
}

export default baseService;