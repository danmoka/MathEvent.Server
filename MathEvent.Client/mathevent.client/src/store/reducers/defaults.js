export const onPendingDefault = (state) => {
    state.isFetching = true;
    state.hasError = false;
};

export const onFulfilledDefault = (state, hasError) => {
    state.isFetching = false;
    state.hasError = hasError;
};

export const onRejectedDefault = (state) => {
    state.isFetching = false;
    state.hasError = true;
};