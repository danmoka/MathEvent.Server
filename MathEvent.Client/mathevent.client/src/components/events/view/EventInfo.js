import React, { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import { patchEvent } from "../../../store/actions/event";
import { useCurrentUser } from "../../../hooks";
import { Date } from "../../_common/Date";
import { Icon, iconTypes } from "../../_common/Icon";
import Button from "../../_common/Button";
import EventFiles from "./EventFiles";
import Image from "../../_common/Image";
import Loader from "../../_common/Loader";
import images from "../../../constants/images";

const EventInfo = () => {
    const dispatch = useDispatch();
    const { eventInfo, isFetchingEvent } = useSelector(state => state.event);
    const { isDarkTheme } = useSelector(state => state.app);
    const { userInfo } = useCurrentUser();

    const [image, setImage] = useState(isDarkTheme ? images.eventDefaultDark : images.eventDefault);
    const [name, setName] = useState("");
    const [applicationUsers, setApplicationUsers] = useState([]);
    const [startDate, setStartDate] = useState(Date.now);
    const [description, setDesctiption] = useState("");
    const [organizationName, setOrganizationName] = useState("Отсутствует")
    const [subscribed, setSubscribed] = useState(false);
    const [eventId, setEventId] = useState(null);

    useEffect(() => {
        if (eventInfo) {
            setApplicationUsers(eventInfo.applicationUsers);
            setSubscribed(eventInfo ? eventInfo.applicationUsers.filter((user) => user.id == userInfo.sub).length > 0 : false);
            setName(eventInfo.name);
            setStartDate(eventInfo.startDate);
            setDesctiption(eventInfo.description ? eventInfo.description : "Отсутствует");
            setOrganizationName(eventInfo.organization ? eventInfo.organization.name : "Отсутствует");
            setEventId(eventInfo.id);
        }
    }, [dispatch, eventInfo]);

    useEffect(() => {
        setImage(isDarkTheme ? images.eventDefaultDark : images.eventDefault);
    }, [dispatch, isDarkTheme])

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

    const handleSubcribersChange = useCallback(() => {
        let subscribersIds = applicationUsers.map((user) => user.id);

        if (subscribersIds.includes(userInfo.sub)) {
            subscribersIds.splice(subscribersIds.indexOf(userInfo.sub), 1);
        }
        else {
            subscribersIds = [...subscribersIds, userInfo.sub];
        }

        handlePatchEvent([
            {
                value: subscribersIds,
                path: "/ApplicationUsers",
                op: "replace"
            }
        ]);
    }, [handlePatchEvent, userInfo, applicationUsers]);

    return (
        isFetchingEvent
        ? (<Loader className="event-grid__loader" size="medium"/>)
        : (eventInfo
            ? (
                <div className="event-info">
                    <div className="event-info__info">
                        <Paper className="event-info__info--main">
                            <Image
                                className="event-info__image"
                                src={image}
                                alt={name}/>
                            <Typography variant="h5">{name}</Typography>
                        </Paper>
                        <Paper className="event-info__info--subscribe">
                            <div className="event-info__horizontal_icon_text">
                                <Icon type={iconTypes.supervisedUser}/>
                                <Typography variant="body1">{applicationUsers.length}</Typography>
                            </div>
                            <Button
                                startIcon={subscribed ? iconTypes.personAddDisabled : iconTypes.personAdd}
                                onClick={handleSubcribersChange}>
                                    {subscribed ? "Вы пойдете" : "Вы не пойдете"}
                                </Button>
                        </Paper>
                        <Paper className="event-info__info--description">
                            <div className="event-info__horizontal_icon_text">
                                <Icon type={iconTypes.accessTime}/>
                                <Date
                                    date={startDate}
                                    variant="body1"/>
                            </div>
                            <div className="event-info__horizontal_icon_text">
                                <Icon type={iconTypes.description}/>
                                <Typography variant="body1" gutterBottom>{description}</Typography>
                            </div>
                            <div className="event-info__horizontal_icon_text">
                                <Icon type={iconTypes.business}/>
                                <Typography variant="body1" gutterBottom>{`${organizationName}`}</Typography>
                            </div>
                        </Paper>
                    </div>
                    <div className="event-info__files">
                        <EventFiles/>
                    </div>
                </div>
            )
            : (<></>)
        )
    );
};

export default EventInfo;