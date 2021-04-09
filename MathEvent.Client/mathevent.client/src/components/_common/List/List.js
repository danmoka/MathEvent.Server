import React from "react";
import ListItem from "./ListItem";

const List = ({ className, items }) => (
    <ul className={className}>
        {items.map((item) => (
            <ListItem
                key={item.id}
                primaryText={item.primaryText}
                secondaryText={item.secondaryText}
                isSelected={item.isSelected}
                index={item.index}
                onClick={item.onClick}
                actions={item.actions}
            />
        ))}
    </ul>
);

export default List;