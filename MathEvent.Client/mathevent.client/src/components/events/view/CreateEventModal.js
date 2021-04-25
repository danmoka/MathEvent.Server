import React, { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Typography } from "@material-ui/core";
import { createEvent } from "../../../store/actions/event";
import { fetchOrganizations } from "../../../store/actions/organization";
import { CreateModal, modalSizes } from "../../_common/Modal";
import { DateField } from "../../_common/Date";
import Checkbox from "../../_common/Checkbox";
import Dropdown from "../../_common/Dropdown";
import TextField from "../../_common/TextField";
import colors from "../../../constants/colors";

const prepareOrganizations = (organizations) =>
    [{ value: "", name: "Без организации" }, ...organizations.map((organization) => ({
    value: organization.id,
    name: organization.name
}))];

const CreateEventModal = () => {
    const dispatch = useDispatch();
    const { organizations } = useSelector((state) => state.organization);
    const { crumbs } = useSelector((state) => state.event);
    const preparedOrganizations = prepareOrganizations(organizations);
    const parent = crumbs.length > 0 ? crumbs[crumbs.length - 1] : null;

    const [name, setName] = useState("");
    const [startDate, setStartDate] = useState(new Date(Date.now()));
    const [description, setDesctiption] = useState("");
    const [organization, setOrganization] = useState(preparedOrganizations[0].value);
    const [hierarchy, setHierarchy] = useState(false);

    useEffect(() => {
        dispatch(fetchOrganizations())
    }, []);

    const handleNameValueChange = useCallback((newName) => {
        setName(newName);
    }, []);

    const handleDescriptionValueChange = useCallback((newDescription) => {
        setDesctiption(newDescription);
    }, []);

    const handleDateValueChange = useCallback((newStartDate) => {
        setStartDate(newStartDate);
    }, []);

    const handleOrganizationChange = useCallback((newOrganization) => {
        setOrganization(newOrganization);
    }, []);

    const handleHierarchyValueChange = useCallback((newValue) => {
        setHierarchy(newValue);
    }, []);

    const handleCreate = useCallback(() => {
        const event = {
            "Name": name,
            "StartDate": startDate,
            "Description": description,
            "OrganizationId": organization,
            "ParentId": parent ? parent.id : null,
            "Hierarchy": hierarchy
        };

        dispatch(createEvent({ event }));
      }, [ dispatch, name, startDate, description, organization]);

    return (
        <CreateModal title="Новое событие" size={modalSizes.small} onCreate={handleCreate}>
            <Typography
                className="event-form__control"
                color={colors.primary}
                variant="body1">
                    {`Событие будет создано в ${parent ? parent.name : "Корне"}`}
            </Typography>
            <Checkbox
                className="event-form__checkbox"
                label="Является множеством других событий"
                value={hierarchy}
                onChange={handleHierarchyValueChange}/>
            <TextField
                className="event-form__control"
                label="Название"
                value={name}
                onChange={handleNameValueChange}
                />
            <TextField
                className="event-form__control"
                label="Описание"
                value={description}
                onChange={handleDescriptionValueChange}
            />
            <DateField
                className="event-form__control"
                value={new Date(startDate)}
                onChange={handleDateValueChange}
                label="Дата и время начала"/>
            <Dropdown
                className="event-form__control"
                label="Организация"
                value={organization}
                items={preparedOrganizations}
                onChange={handleOrganizationChange}
            />
        </CreateModal>
    );
};

export default CreateEventModal;