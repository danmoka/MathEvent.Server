import { useSelector } from "react-redux";

const useCurrentUser = () => {
    const { userInfo, hasToken, isAuthenticated, isFetching } = useSelector(state => state.account);

    return { userInfo, hasToken, isAuthenticated, isFetching };
};

export default useCurrentUser;