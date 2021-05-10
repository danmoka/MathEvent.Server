import { createAsyncThunk } from "@reduxjs/toolkit";
import { OpenStreetMapProvider } from 'leaflet-geosearch';

export const fetchPosition = createAsyncThunk("fetchPosition", async (location) => {
    const provider = new OpenStreetMapProvider();
    const results = await provider.search({ query: location });

    return { results };
});