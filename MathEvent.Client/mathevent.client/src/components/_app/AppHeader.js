import React, { useState } from "react";
import { useDispatch } from "react-redux";
import AppBar from '@material-ui/core/AppBar';
import ListItemIcon from "@material-ui/core/ListItemIcon";
import MenuItem from "@material-ui/core/MenuItem";
import Popover from "@material-ui/core/Popover";
import Toolbar from '@material-ui/core/Toolbar';

import { useCurrentUser } from "../../hooks";
import { logout, revocation } from "../../store/actions/account";
import Button, { colors } from "../_common/Button";
import { Icon, iconTypes } from "../_common/Icon";
import { navigateToHome, navigateToLogin } from "../../utils/navigator";

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
        <AppBar position="static">
            <Toolbar className="app-header">
                <section className="app-header__section">
                    <Button
                        className="app-header__button"
                        startIcon={iconTypes.home}
                        onClick={navigateToHome}
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
                                    onClick={() => {console.log("register button clicked");}}
                                >
                                    Регистрация
                                </Button>
                                <Button
                                    className="app-header__button"
                                    onClick={handleLoginClick}
                                >
                                    Войти
                                </Button>
                            </>
                        ) : (
                            isFetching
                                ? (<div>Ожидайте...</div>)
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
            </Toolbar>
        </AppBar>
    );
};

export default AppHeader;