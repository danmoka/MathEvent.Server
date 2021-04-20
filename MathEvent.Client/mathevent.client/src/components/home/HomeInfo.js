import React from "react";
import Grid from "@material-ui/core/Grid";
import Info from "./Info";

const HomeInfo = () => (
    <Grid
        container
        spacing={2}
        direction="row"
        justifyContent="flex-start"
        alignItems="start">
        <Grid item xs>
            <Info title="События" text="Создавайте свои события и находите интересующие!"/>
        </Grid>
        <Grid item xs>
            <Info title="Материалы" text="Прикрепляйте материалы прямо к вашим событиям! Материалы будут доступны только авторизованным пользователям."/>
        </Grid>
        <Grid item xs>
            <Info title="Статистика" text="Изучайте статистику по событиям с помощью наших удобных схем и графиков!"/>
        </Grid>
    </Grid>
);

export default HomeInfo;