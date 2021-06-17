import { createSlice } from "@reduxjs/toolkit";
import { showModal, hideModal } from "../actions/modal";

const initialState = {
    modalType: null,
    modalProps: null
};

const modalSlice = createSlice({
    name: "modalSlice",
    initialState: initialState,
    extraReducers: {
        [showModal]: (state, { payload: { modalType, modalProps } }) => {
            state.modalType = modalType;
            state.modalProps = modalProps;
        },
        [hideModal]: (state) => {
            state.modalType = null;
            state.modalProps = null;
        }
    }
});

export default modalSlice.reducer;