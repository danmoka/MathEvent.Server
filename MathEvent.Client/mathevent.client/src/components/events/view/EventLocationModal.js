import React from "react";
import { useSelector } from "react-redux";
import Typography from "@material-ui/core/Typography";
import { ShowModal, modalSizes } from "../../_common/Modal";
import Location from "../../_common/Map";

const EventLocationModal = () => {
    const { position } = useSelector((state) => state.modal.modalProps);

    return (
        <ShowModal title="Местоположение" size={modalSizes.medium}>
            {position
                ? (<Location location={[position.y, position.x]} label={position.label}/>)
                : (<Typography variant="body1">Не удалось определить местоположение</Typography>)}
        </ShowModal>
    );
};

export default EventLocationModal;