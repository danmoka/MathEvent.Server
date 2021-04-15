import { createAsyncThunk } from "@reduxjs/toolkit";
import organizationService from "../../api/services/organization-service";
import statusCode from "../../utils/status-code-reader";

export const fetchOrganizations = createAsyncThunk("fetchOrganizations", async () => {
    const response = await organizationService.fetchOrganizations();

    if (statusCode(response).ok) {
        const organizations = await response.json();

        return { organizations };
    }

    return { organizations: [] };
});