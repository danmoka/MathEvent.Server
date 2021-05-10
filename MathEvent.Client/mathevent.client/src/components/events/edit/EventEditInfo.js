import React, { useEffect, useState, useCallback }from "react";
import { useDispatch, useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import { fetchOrganizations } from "../../../store/actions/organization";
import { patchEvent, showUploadEventAvatarModal } from "../../../store/actions/event";
import { getImageSrc } from "../../../utils/get-image-src";
import { DateField } from "../../_common/Date";
import { iconTypes } from "../../_common/Icon";
import Button from "../../_common/Button";
import Checkbox from "../../_common/Checkbox";
import Dropdown from "../../_common/Dropdown";
import Image from "../../_common/Image";
import TextField from "../../_common/TextField";
import images from "../../../constants/images";

const prepareOrganizations = (organizations) =>
    [{ value: "", name: "Без организации" }, ...organizations.map((organization) => ({
    value: organization.id,
    name: organization.name
}))];

const EventEditInfo = () => {
    const dispatch = useDispatch();
    const { eventInfo: event } = useSelector((state) => state.event);
    const { organizations } = useSelector((state) => state.organization);
    const { isDarkTheme } = useSelector(state => state.app);
    const preparedOrganizations = prepareOrganizations(organizations);

    const [eventId, setEventId] = useState(null);
    const [avatarPath, setAvatarPath] = useState(null);
    const [name, setName] = useState("");
    const [startDate, setStartDate] = useState(event ? event.startDate : new Date(Date.now()));
    const [location, setLocation] = useState("");
    const [description, setDesctiption] = useState("");
    const [organization, setOrganization] = useState(preparedOrganizations[0].value);
    const [hierarchy, setHierarchy] = useState(false);

    useEffect(() => {
        dispatch(fetchOrganizations())
    }, []);

    useEffect(() => {
        if (event) {
            setEventId(event.id);
            setName(event.name);
            setStartDate(event.startDate);
            setLocation(event.location);
            setDesctiption(event.description);
            setHierarchy(event.hierarchy ? true : false);

            if (event.organization) {
                setOrganization(event.organization.id);
            }

            event.avatarPath
                ? setAvatarPath(event.avatarPath)
                : setAvatarPath(null);
        }
    }, [event]);

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

    const handleLocationValueChange = useCallback((newLocation) => {
        setLocation(newLocation);
        handlePatchEvent([
            {
                value: newLocation,
                path: "/Location",
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
                value: newValue ? newValue : null,
                path: "/Hierarchy",
                op: "replace"
            }
        ]);
    }, [handlePatchEvent, event]);

    const handleEventAvatarUpload = useCallback(() => {
        dispatch(showUploadEventAvatarModal({ eventId: eventId }));
    }, [dispatch, eventId]);

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
                <TextField
                    className="event-edit-form__control"
                    label="Место"
                    value={location}
                    onChange={handleLocationValueChange}
                />
                <Dropdown
                    className="event-edit-form__control"
                    label="Организация"
                    value={organization}
                    items={preparedOrganizations}
                    onChange={handleOrganizationChange}
                />
            </section>
            <section className="event-edit-info__section--image">
                <Image
                    className="event-edit-info__image"
                    src={avatarPath
                        ? getImageSrc(avatarPath)
                        : (isDarkTheme ? images.eventDefaultDark : images.eventDefault)}
                    alt={name}/>
                <Button
                    className="event-edit-form__control"
                    startIcon={iconTypes.upload}
                    onClick={handleEventAvatarUpload}>
                        Загрузить изображение
                </Button>
            </section>
        </Paper>
    );
};

export default EventEditInfo;