import { createSlice } from "@reduxjs/toolkit";
import {
    onPendingOrganizations,
    onFulfilledOrganizations,
    onRejectedOrganizations,
    onPendingOrganizationStatistics,
    onFulfilledOrganizationStatistics,
    onRejectedOrganizationStatistics
} from "./defaults";
import { fetchOrganizations, fetchStatistics } from "../actions/organization";

const initialState = {
    organizations: [],
    statistics: [],
    isFetchingOrganizations: false,
    isFetchingOrganizationStatistics: false,
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

        [fetchStatistics.pending]: (state) => {
            onPendingOrganizationStatistics(state);
        },
        [fetchStatistics.fulfilled]: (state, { payload: { statistics, hasError } }) => {
            onFulfilledOrganizationStatistics(state, hasError);

            if (!hasError) {
                state.statistics = statistics;
            }
        },
        [fetchStatistics.rejected]: (state) => {
            onRejectedOrganizationStatistics(state);
            state.statistics = [];
        },
    }
});

export default organizationSlice.reducer;