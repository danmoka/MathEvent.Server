import { createSlice } from "@reduxjs/toolkit";
import { onPendingDefault, onFulfilledDefault, onRejectedDefault } from "./defaults";
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
            onPendingDefault(state, state.isFetchingOrganizations);
        },
        [fetchOrganizations.fulfilled]: (state, { payload: { organizations, hasError } }) => {
            onFulfilledDefault(state, hasError, state.isFetchingOrganizations);

            if (!hasError) {
                state.organizations = organizations;
            }
        },
        [fetchOrganizations.rejected]: (state) => {
            onRejectedDefault(state, state.isFetchingOrganizations);
            state.organizations = [];
        },
    }
});

export default organizationSlice.reducer;