import React from "react";
import Breadcrumbs from '@material-ui/core/Breadcrumbs';
import Breadcrumb from "./Breadcrumb";

const CommonBreadcrumbs = ({ className, items }) => (
    <Breadcrumbs 
            className={className}>
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