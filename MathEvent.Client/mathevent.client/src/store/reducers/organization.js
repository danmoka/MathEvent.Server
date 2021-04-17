import { createSlice } from "@reduxjs/toolkit";
import { onPendingOrganizations, onFulfilledOrganizations, onRejectedOrganizations } from "./defaults";
import { fetchOrganizations} from "../actions/organization";

const initialState = {
    organizations: [],
    isFetchingOrganizations: false,
    hasError: false
};

const organizationSlice = createSlice({
    name: "organizationSlice",
    initialState: initialState,
    extraReducers: {
        [fetchOrganizations.pending]: (state) => {
            onPendingOrganizations(state);
        },
        [fetchOrganizations.fulfilled]: (state, { payload: { organizations, hasError } }) => {
            onFulfilledOrganizations(state, hasError);

            if (!hasError) {
                state.organizations = organizations;
            }
        },
        [fetchOrganizations.rejected]: (state) => {
            onRejectedOrganizations(state);
            state.organizations = [];
        },
    }
});

export default organizationSlice.reducer;