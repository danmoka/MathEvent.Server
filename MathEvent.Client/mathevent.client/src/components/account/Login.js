import React, { useState } from "react";
import { useDispatch } from "react-redux";
import Paper from "@material-ui/core/Paper";
import { fetchTokens } from "../../store/actions/account";
import { useTitle } from "../../hooks";
import Button from "../_common/Button";
import TextField from "../_common/TextField";
import "./Account.scss";

const Login = () => {
    const dispatch = useDispatch();
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");

    const clearFields = () => {
        setUserName("");
        setPassword("");
    };

    const handleUserNameChange = (value) => setUserName(value);
    const handlePasswordChange = (value) => setPassword(value);

    const handleSubmit = () => {
        const credentials = { userName, password };
        dispatch(fetchTokens(credentials));
        clearFields();
    };

    useTitle("Вход");

    return (
        <Paper className="account-form">
            <TextField value={userName} onChange={handleUserNameChange} label="Логин"/>
            <TextField value={password} onChange={handlePasswordChange} label="Пароль" type="password"/>
            <Button onClick={handleSubmit}>Войти</Button>
        </Paper>
    );
};

export default Login;