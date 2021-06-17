import React from "react";
import { useDispatch } from "react-redux";
import { logout } from "../../store/actions/account";
import { InfoModal } from "../_common/Modal";

const LogoutModal = () => {
    const dispatch = useDispatch();
    const handleLogout = () => dispatch(logout());

    return (
        <InfoModal
            infoText="Вы действительно хотите выйти?"
            okButtonText="Да"
            title="Выход"
            onSubmit={handleLogout}
        />
    );
};

export default LogoutModal;