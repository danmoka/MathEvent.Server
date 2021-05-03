import React, { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import Paper from "@material-ui/core/Paper";
import TextField from "@material-ui/core/TextField";
import { register } from "../../store/actions/user";
import { fetchOrganizations } from "../../store/actions/organization";
import Button from "../_common/Button";
import Dropdown from "../_common/Dropdown";
import "./Account.scss";

const prepareOrganizations = (organizations) =>
    [{ value: "", name: "Без организации" }, ...organizations.map((organization) => ({
    value: organization.id,
    name: organization.name
}))];

const Register = () => {
    const dispatch = useDispatch();
    const { organizations } = useSelector((state) => state.organization);
    const preparedOrganizations = prepareOrganizations(organizations);

    const [email, setEmail] = useState("");
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [name, setName] = useState("");
    const [surname, setSurname] = useState("");
    const [organization, setOrganization] = useState(preparedOrganizations[0].value);

    useEffect(() => {
        clearFields();
        dispatch(fetchOrganizations())
    }, []);

    const clearFields = () => {
        setEmail("");
        setUserName("");
        setPassword("");
        setConfirmPassword("");
        setName("");
        setSurname("");
        setOrganization(preparedOrganizations[0].value);
    };

    const handleEmailChange = (e) => setEmail(e.target.value);
    const handleUserNameChange = (e) => setUserName(e.target.value);
    const handlePasswordChange = (e) => setPassword(e.target.value);
    const handleConfirmPasswordChange = (e) => setConfirmPassword(e.target.value);
    const handleNameChange = (e) => setName(e.target.value);
    const handleSurnameChange = (e) => setSurname(e.target.value);
    const handleOrganizationChange = useCallback((newOrganization) => {
        setOrganization(newOrganization);
    });
    // todo: после регистрации при входе имя не появляется (мб из-за того, что токены были или есть?)

    const handleSubmit = () => {
        const credentials = {
            email,
            userName,
            surname,
            password,
            confirmPassword,
            name,
            surname,
            organizationId: organization ? organization : null
        };
        dispatch(register(credentials));
    };

    return (
        <Paper className="account-form">
            <TextField value={email} onChange={handleEmailChange} label="Email"/>
            <TextField value={userName} onChange={handleUserNameChange} label="Логин"/>
            <TextField value={name} onChange={handleNameChange} label="Имя"/>
            <TextField value={surname} onChange={handleSurnameChange} label="Фамилия"/>
            <TextField value={password} onChange={handlePasswordChange} label="Пароль" type="password"/>
            <TextField value={confirmPassword} onChange={handleConfirmPasswordChange} label="Повторите пароль" type="password"/>
            <Dropdown
                    label="Организация"
                    value={organization}
                    items={preparedOrganizations}
                    onChange={handleOrganizationChange}
                />
            <Button onClick={handleSubmit}>Регистрация</Button>
        </Paper>
    );
};

export default Register;