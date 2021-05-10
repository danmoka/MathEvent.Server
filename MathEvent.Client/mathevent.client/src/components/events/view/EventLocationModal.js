import React, { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import Typography from "@material-ui/core/Typography";
import { ShowModal, modalSizes } from "../../_common/Modal";
import Location from "../../_common/Map";
import Loader from "../../_common/Loader";

const EventLocationModal = () => {
    const { positionResults, isFetchingPosition } = useSelector((state) => state.map);
    const [position, setPosition] = useState([]);
    const [label, setLabel] = useState("");

    useEffect(() => {
        if (positionResults.length > 0) {
            const positionResult = positionResults[0];
            setPosition([positionResult.y, positionResult.x]);
            setLabel(positionResult.label);
        }
    }, [positionResults]);

    return (
        isFetchingPosition
        ? (<Loader size="medium"/>)
        : (
            <ShowModal title="Местоположение" size={modalSizes.medium}>
                {position.length > 0
                    ? (<Location location={position} label={label}/>)
                    : (<Typography variant="body1">Не удалось определить местоположение</Typography>)}
            </ShowModal>
        )
    );
};

export default EventLocationModal;