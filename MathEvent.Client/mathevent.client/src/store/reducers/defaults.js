export const onPendingDefault = (state, isFetching) => {
    isFetching = true;
    state.hasError = false;
};

export const onFulfilledDefault = (state, hasError, isFetching) => {
    isFetching = false;
    state.hasError = hasError;
};

export const onRejectedDefault = (state, isFetching) => {
    isFetching = false;
    state.hasError = true;
};