const defaultOnPendingFetchValue = false;

const onPendingDefault = (state) => {
    state.hasError = false;
};

const onFulfilledDefault = (state, hasError) => {
    state.hasError = hasError;
};

const onRejectedDefault = (state) => {
    state.hasError = true;
};

// Events
export const onPendingEvents = (state) => {
    state.isFetchingEvents = defaultOnPendingFetchValue;
    onPendingDefault(state);
};

export const onFulfilledEvents = (state, hasError) => {
    state.isFetchingEvents = false;
    onFulfilledDefault(state, hasError);
};

export const onRejectedEvents = (state) => {
    state.isFetchingEvents = false;
    onRejectedDefault(state);
};

// Event
export const onPendingEvent = (state) => {
    state.isFetchingEvent = defaultOnPendingFetchValue;
    onPendingDefault(state);
};

export const onFulfilledEvent = (state, hasError) => {
    state.isFetchingEvent = false;
    onFulfilledDefault(state, hasError);
};

export const onRejectedEvent = (state) => {
    state.isFetchingEvent = false;
    onRejectedDefault(state);
};

// EventBreadcrumbs
export const onPendingEventBreadcrumbs = (state) => {
    state.isFetchingEventBreadcrumbs = defaultOnPendingFetchValue;
    onPendingDefault(state);
};

export const onFulfilledEventBreadcrumbs = (state, hasError) => {
    state.isFetchingEventBreadcrumbs = false;
    onFulfilledDefault(state, hasError);
};

export const onRejectedEventBreadcrumbs = (state) => {
    state.isFetchingEventBreadcrumbs = false;
    onRejectedDefault(state);
};

// Organizations
export const onPendingOrganizations = (state) => {
    state.isFetchingOrganizations = defaultOnPendingFetchValue;
    onPendingDefault(state);
};

export const onFulfilledOrganizations = (state, hasError) => {
    state.isFetchingOrganizations = false;
    onFulfilledDefault(state, hasError);
};

export const onRejectedOrganizations = (state) => {
    state.isFetchingOrganizations = false;
    onRejectedDefault(state);
};