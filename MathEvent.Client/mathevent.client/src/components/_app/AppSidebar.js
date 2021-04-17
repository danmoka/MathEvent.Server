import React from "react";
import { useSelector } from "react-redux";
import { IconButton, iconTypes } from "../_common/Icon";
import { navigateToEvents } from "../../utils/navigator";
import routes from "../../utils/routes";

const AppSidebar = () => {
    const currentRoute = useSelector(state => state.router.location.pathname);
    const isSelected = (route) => currentRoute.includes(route);

    return (
        <div className="app-sidebar bg-light">
            <div className="app-sidebar__items">
                <IconButton
                    type={iconTypes.events}
                    selected={isSelected(routes.events)}
                    onClick={navigateToEvents}
                />
            </div>
        </div>
    );
};

export default AppSidebar;