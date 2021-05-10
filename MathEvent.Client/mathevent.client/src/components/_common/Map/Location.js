import React from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import "./Map.scss";

const Location = ({ location, label, zoom=13 }) => {
    return (
        <MapContainer
            className="map-location"
            center={location}
            zoom={zoom}
        >
            <Marker
                position={location}>
                <Popup>
                    {label}
                </Popup>
            </Marker>
            <TileLayer
                attribution='&amp;copy <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
        </MapContainer>
    );
};

export default Location;