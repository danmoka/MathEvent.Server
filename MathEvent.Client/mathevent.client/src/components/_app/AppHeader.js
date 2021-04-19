import React, { useState } from "react";
import { useDispatch } from "react-redux";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import MenuItem from "@material-ui/core/MenuItem";
import Popover from "@material-ui/core/Popover";

import { useCurrentUser } from "../../hooks";
import { logout, revocation } from "../../store/actions/account";
import Button, { colors } from "../_common/Button";
import { Icon, iconTypes } from "../_common/Icon";
import { navigateToLogin } from "../../utils/navigator";

const AppHeader = () => {
    const dispatch = useDispatch();
    const { userInfo, isAuthenticated, isFetching } = useCurrentUser();
    const [anchorEl, setAnchorEl] = useState(null);

    const handleMenuOpen = (event) => setAnchorEl(event.currentTarget);
    const handleMenuClose = () => setAnchorEl(null);

    const handleLoginClick = () => navigateToLogin();
    const handleLogoutClick = () => {
        dispatch(logout());
        dispatch(revocation());
        handleMenuClose();
    };

    return (
        <header className="app-header">
            <section className="app-header__section">
                <Button
                    className="app-header__button"
                    color={colors.transparentBlack}
                    startIcon={iconTypes.home}
                    onClick={() => {console.log("home button clicked");}}
                >
                    MathEvent
                </Button>
            </section>
            <section className="app-header__section">
                {!isAuthenticated
                    ? (
                        <>
                            <Button
                                className="app-header__button"
                                color={colors.transparentBlack}
                                onClick={() => {console.log("register button clicked");}}
                            >
                                Регистрация
                            </Button>
                            <Button
                                className="app-header__button"
                                color={colors.transparentBlack}
                                onClick={handleLoginClick}
                            >
                                Войти
                            </Button>
                        </>
                    ) : (
                        isFetching
                            ? (<div>Загрузка...</div>)
                            : (
                                <>
                                    <Button
                                        className="app-header__button"
                                        color={colors.transparentBlack}
                                        endIcon={iconTypes.account}
                                        onClick={handleMenuOpen}
                                    >
                                        {userInfo.email}
                                    </Button>
                                    <Popover
                                        id="app-header-popover"
                                        open={Boolean(anchorEl)}
                                        anchorEl={anchorEl}
                                        onClose={handleMenuClose}
                                        anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
                                        transformOrigin={{ vertical: "top", horizontal: "right" }}
                                    >
                                        <div className="app-header__menu">
                                            <MenuItem onClick={handleMenuClose}>
                                                {userInfo.email} {userInfo.name}
                                            </MenuItem>
                                            <div className="app-header__menu-divider"/>
                                            <MenuItem onClick={handleMenuClose}>
                                                <ListItemIcon>
                                                    <Icon type={iconTypes.stats}/>
                                                </ListItemIcon>
                                                Статистика
                                            </MenuItem>
                                            <MenuItem onClick={handleMenuClose}>
                                                <ListItemIcon>
                                                    <Icon type={iconTypes.settings}/>
                                                </ListItemIcon>
                                                Настройки
                                            </MenuItem>
                                            <MenuItem onClick={handleLogoutClick}>
                                                <ListItemIcon>
                                                    <Icon type={iconTypes.logout}/>
                                                </ListItemIcon>
                                                Выйти
                                            </MenuItem>
                                        </div>
                                    </Popover>
                                </>
                            )
                        )}
            </section>
        </header>
    );
};

export default AppHeader;