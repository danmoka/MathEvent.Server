import { createAction } from "@reduxjs/toolkit";

export const setHeader = createAction("setHeader", (header) => ({ payload: { header } }));