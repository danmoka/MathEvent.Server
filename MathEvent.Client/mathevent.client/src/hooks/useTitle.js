import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { setHeader } from "../store/actions/app";

const useTitle = (title) => {
    const dispatch = useDispatch();

    useEffect(() => {
        if (!title) {
            document.title = "MathEvent";

            return;
        }

        if (title === "MathEvent")
            document.title = "Главная";
        else
            document.title = `${title}`;

        dispatch(setHeader(title));
    }, [dispatch, title]);
};

export default useTitle;