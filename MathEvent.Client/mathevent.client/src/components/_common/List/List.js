import React from "react";
import Divider from '@material-ui/core/Divider';
import ListItem from "./ListItem";

const List = ({ className, items }) => (
    <ul className={className}>
        {items.map((item) => (
            <div key={item.id}>
                <ListItem
                    
                    primaryText={item.primaryText}
                    secondaryText={item.secondaryText}
                    isSelected={item.isSelected}
                    index={item.index}
                    onClick={item.onClick}
                    actions={item.actions}
                />
                <Divider/>
            </div>
        ))}
    </ul>
);

export default List;