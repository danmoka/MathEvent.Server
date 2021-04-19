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

// Files
export const onPendingFiles = (state) => {
    state.isFetchingFiles = defaultOnPendingFetchValue;
    onPendingDefault(state);
};

export const onFulfilledFiles = (state, hasError) => {
    state.isFetchingFiles = false;
    onFulfilledDefault(state, hasError);
};

export const onRejectedFiles = (state) => {
    state.isFetchingFiles = false;
    onRejectedDefault(state);
};

// File
export const onPendingFile = (state) => {
    state.isFetchingFile = defaultOnPendingFetchValue;
    onPendingDefault(state);
};

export const onFulfilledFile = (state, hasError) => {
    state.isFetchingFile = false;
    onFulfilledDefault(state, hasError);
};

export const onRejectedFile = (state) => {
    state.isFetchingFile = false;
    onRejectedDefault(state);
};

// FileBreadcrumbs
export const onPendingFileBreadcrumbs = (state) => {
    state.isFetchingFileBreadcrumbs = defaultOnPendingFetchValue;
    onPendingDefault(state);
};

export const onFulfilledFileBreadcrumbs = (state, hasError) => {
    state.isFetchingFileBreadcrumbs = false;
    onFulfilledDefault(state, hasError);
};

export const onRejectedFileBreadcrumbs = (state) => {
    state.isFetchingFileBreadcrumbs = false;
    onRejectedDefault(state);
};

// Account
export const onPendingAccount = (state) => {
    state.isFetchingAccount = defaultOnPendingFetchValue;
    onPendingDefault(state);
};

export const onFulfilledAccount = (state, hasError) => {
    state.isFetchingAccount = false;
    onFulfilledDefault(state, hasError);
};

export const onRejectedAccount = (state) => {
    state.isFetchingAccount = false;
    onRejectedDefault(state);
};