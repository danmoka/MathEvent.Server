import { getAccessToken } from "../../utils/local-storage-manager";

const baseService = {
    get: async (url) => {
        try {
            return await fetch(url, {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${getAccessToken()}`
                }
            });
        } catch (e) {
            console.log(e);
        }
    },
    post: async (url, data) => {
        try {
          return await fetch(url, {
            method: "POST",
            body: JSON.stringify(data),
            headers: {
              Authorization: `Bearer ${getAccessToken()}`,
              "Content-Type": "application/json",
            },
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
                    "Authorization": `Bearer ${getAccessToken()}`,
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
                    "Authorization": `Bearer ${getAccessToken()}`,
                    "Content-Type": "application/json"
                }
            });
        } catch (e) {
            console.log(e);
        }
    },
    delete: async (url) => {
        try {
            return await fetch(url, {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${getAccessToken()}`
                }
            });
        } catch (e) {
            console.log(e);
        }
    }
};

const accountBaseService = {
    post: async (url, data) => {
        try {
            return await fetch(url, {
                method: "POST",
                body: new URLSearchParams(data),
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                }
            });
        } catch (e) {
            console.log(e);
        }
    },
};

export { baseService, accountBaseService}