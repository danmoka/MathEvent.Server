const statusCode = (response) => {
    const checkStatusCode = (expected) => response.status === expected;

    return {
        ok: checkStatusCode(200),
        created: checkStatusCode(201),
        noContent: checkStatusCode(204),
        badRequest: checkStatusCode(400),
        unauthorized: checkStatusCode(401),
        forbidden: checkStatusCode(403),
        internalServerError: checkStatusCode(500)
    };
};

export default statusCode;