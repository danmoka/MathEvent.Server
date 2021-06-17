import React from "react";
import { useDispatch } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Typography from "@material-ui/core/Typography";

import Button, { buttonTypes } from "../Button";
import { IconButton, iconTypes } from "../Icon";
import { hideModal } from "../../../store/actions/modal";
import modalSizes from "../../../constants/modal-sizes";
import "./Modal.scss";

const ShowModal = ({ children, size = modalSizes.small, title }) => {
    const dispatch = useDispatch();
    const handleClose = () => dispatch(hideModal());

    return (
        <Dialog maxWidth={size} onClose={handleClose} fullWidth open>
            <DialogTitle className="modal-common__header" disableTypography>
                <Typography variant="h6" gutterBottom>{title}</Typography>
                <IconButton type={iconTypes.close} onClick={handleClose}/>
            </DialogTitle>
            <DialogContent className="modal-common__content"  dividers>{children}</DialogContent>
            <DialogActions>
                <Button type={buttonTypes.text} onClick={handleClose}>Назад</Button>
            </DialogActions>
        </Dialog>
    );
};

export default ShowModal;