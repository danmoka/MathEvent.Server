import { createAction } from "@reduxjs/toolkit";

export const showModal = createAction("showModal", (modalType, modalProps) => ({ payload: { modalType, modalProps } }));
export const hideModal = createAction("hideModal");