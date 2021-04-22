import React from "react";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import { IconButton, iconTypes } from "../Icon";
import Breadcrumb from "./Breadcrumb";

const CommonBreadcrumbs = ({ className, items, backButtonOnClick }) => (
    <Breadcrumbs
            aria-label="breadcrumb"
            className={className}>
            <IconButton
                type={iconTypes.back}
                onClick={backButtonOnClick}
            />
            {items.map((item) => (
                <Breadcrumb
                    key={item.id}
                    primaryText={item.primaryText}
                    isLast={item.isLast}
                    index={item.index}
                    onClick={item.onClick}
                />))}
    </Breadcrumbs>
);

export default CommonBreadcrumbs;