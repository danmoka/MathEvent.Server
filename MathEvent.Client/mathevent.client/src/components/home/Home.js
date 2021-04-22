import React from 'react';
import { useSelector } from 'react-redux';
import Grid from '@material-ui/core/Grid';
import Paper from "@material-ui/core/Paper";
import Typography from '@material-ui/core/Typography';
import HomeInfo from "./HomeInfo";
import Image from "../_common/Image";
import colors from "../../constants/colors";
import images from "../../constants/images";

const Home = () => {
    const { isDarkTheme } = useSelector(state => state.app);

    return (
        <Paper className="home-container">
            <Grid
                container
                spacing={2}
                direction="row"
                justify="flex-start"
                alignItems="flex-start">
                <Grid item xs={6}>
                    <Typography  variant="h2" gutterBottom color={colors.primary} align="center" gutterBottom>
                        MathEvent
                    </Typography>
                    <Typography variant="h5" align="center" paragraph>
                        MathEvent - это система поддержки публичных мероприятий.
                        Здесь вы можете найти интересующие Вас события,
                        а также создать свои!
                    </Typography>
                    <div className="home-info">
                        <HomeInfo/>
                    </div>
                </Grid>
                <Grid item xs={6}>
                <Image
                    className="home-logo"
                    src={isDarkTheme ? images.homeDark : images.home}
                    alt="YSU Math"
                />
                </Grid>
            </Grid>
        </Paper>
    );
};

export default Home;