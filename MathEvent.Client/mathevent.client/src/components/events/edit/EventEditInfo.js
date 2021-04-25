import React, { useEffect, useState, useCallback }from "react";
import { useDispatch, useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import { fetchOrganizations } from "../../../store/actions/organization";
import { patchEvent } from "../../../store/actions/event";
import { DateField } from "../../_common/Date";
import Checkbox from "../../_common/Checkbox";
import Dropdown from "../../_common/Dropdown";
import TextField from "../../_common/TextField";

const prepareOrganizations = (organizations) =>
    [{ value: "", name: "Без организации" }, ...organizations.map((organization) => ({
    value: organization.id,
    name: organization.name
}))];

const EventEditInfo = () => {
    const dispatch = useDispatch();
    const { eventInfo: event } = useSelector((state) => state.event);
    const { organizations } = useSelector((state) => state.organization);
    const preparedOrganizations = prepareOrganizations(organizations);

    const [eventId, setEventId] = useState(null);
    const [name, setName] = useState("");
    const [startDate, setStartDate] = useState(event ? event.startDate : new Date(Date.now()));
    const [description, setDesctiption] = useState("");
    const [organization, setOrganization] = useState(preparedOrganizations[0].value);
    const [hierarchy, setHierarchy] = useState(event ? event.hierarchy : false);

    useEffect(() => {
        dispatch(fetchOrganizations())
    }, []);

    useEffect(() => {
        if (event) {
            setEventId(event.id);
            setName(event.name);
            setStartDate(event.startDate);
            setDesctiption(event.description);
            if (event.organization) {
                setOrganization(event.organization.id);
            }
        }
    }, [event?.id]);

    const handlePatchEvent = useCallback(
        (data) => {
            dispatch(
                patchEvent({
                  eventId,
                  data,
                })
              );
        },
        [dispatch, eventId]
    );

    const handleNameValueChange = useCallback((newName) => {
        setName(newName);
        handlePatchEvent([
            {
                value: newName,
                path: "/Name",
                op: "replace"
            }
        ]);
    }, [handlePatchEvent, event]);

    const handleDescriptionValueChange = useCallback((newDescription) => {
        setDesctiption(newDescription);
        handlePatchEvent([
            {
                value: newDescription,
                path: "/Description",
                op: "replace"
            }
        ]);
    }, [handlePatchEvent, event]);

    const handleDateValueChange = useCallback((newStartDate) => {
        setStartDate(newStartDate);
        handlePatchEvent([
            {
                value: newStartDate,
                path: "/StartDate",
                op: "replace"
            }
        ]);
    }, [handlePatchEvent, event]);

    const handleOrganizationChange = useCallback((newOrganization) => {
        setOrganization(newOrganization);
        handlePatchEvent([
            {
                value: newOrganization ? newOrganization : null,
                path: "/OrganizationId",
                op: "replace"
            }
        ]);
    }, [handlePatchEvent, event]);

    const handleHierarchyValueChange = useCallback((newValue) => {
        setHierarchy(newValue);
        handlePatchEvent([
            {
                value: newValue,
                path: "/Hierarchy",
                op: "replace"
            }
        ]);
    }, [handlePatchEvent, event]);

    return (
        <Paper className="event-edit-info">
            <section className="event-edit-info__section--description">
                <Checkbox
                    className="event-edit-form__checkbox"
                    label="Является множеством других событий"
                    value={hierarchy}
                    onChange={handleHierarchyValueChange}/>
                <TextField
                    className="event-edit-form__control"
                    label="Название"
                    value={name}
                    onChange={handleNameValueChange}
                />
                <TextField
                    className="event-edit-form__control"
                    label="Описание"
                    value={description}
                    onChange={handleDescriptionValueChange}
                />
                <DateField
                    className="event-edit-form__control"
                    value={new Date(startDate)}
                    onChange={handleDateValueChange}
                    label="Дата и время начала"/>
                <Dropdown
                    className="event-edit-form__control"
                    label="Организация"
                    value={organization}
                    items={preparedOrganizations}
                    onChange={handleOrganizationChange}
                />
            </section>
            {/* <section className="event-edit-info__section--image">
                <img
                    className="event-edit-info__image"
                    src="https://vancouverhumanesociety.bc.ca/wp-content/uploads/2019/01/Upcoming-eventsiStock-978975308-e1564610924151-1024x627.jpg"
                    alt={eventInfo.name}/>
            </section> */}
        </Paper>
    );
};

export default EventEditInfo;