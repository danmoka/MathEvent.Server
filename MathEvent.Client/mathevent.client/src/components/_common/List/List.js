import React, { useCallback, useState } from "react";
import Divider from '@material-ui/core/Divider';
import ListItem from "./ListItem";

const List = ({ className, items, onCheck }) => {
    const checkedItems = items.filter((item) => item.checked);
    const [checkedItemIds, setCheckedItemIds] = useState(checkedItems.map((item) => item.id));

    const handleCheckItem = useCallback(
        (itemId, isChecked) => {
            let newCheckedItemIds;
            if (isChecked) newCheckedItemIds = [...checkedItemIds, itemId];
            else newCheckedItemIds = checkedItemIds.filter((id) => id !== itemId);

            setCheckedItemIds(newCheckedItemIds);
            onCheck(newCheckedItemIds);
        },
        [checkedItemIds, onCheck]
    );

    return (
        <ul className={className}>
        {items.map((item) => (
            <div key={item.id}>
                <ListItem
                    id={item.id}
                    primaryText={item.primaryText}
                    secondaryText={item.secondaryText}
                    avatarText={item.avatarText}
                    isSelected={item.isSelected}
                    checked={checkedItemIds.includes(item.id)}
                    index={item.index}
                    actions={item.actions}
                    onClick={item.onClick}
                    onCheck={onCheck ? handleCheckItem : null}
                />
                <Divider/>
            </div>
        ))}
    </ul>
    )
};

export default List;