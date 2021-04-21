import React from 'react';
import Grid from '@material-ui/core/Grid';
import Paper from "@material-ui/core/Paper";
import Typography from '@material-ui/core/Typography';
import HomeInfo from "./HomeInfo";
import Image from "../_common/Image";
import colors from "../../constants/colors";
import images from "../../constants/images";

const Home = () => {
    return (
        <div className="home">
            <Paper className="home-container">
                <Grid container spacing={3}>
                    <Grid item xs={6}>
                        <Typography component="h1" variant="h2" color={colors.primary} align="center" gutterBottom>
                            MathEvent
                        </Typography>
                        <Typography variant="h5" color={colors.textSecondary} align="center" paragraph>
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
                        src={images.home}
                        alt="YSU Math"
                    />
                    </Grid>
                </Grid>
            </Paper>
        </div>
    );
};

export default Home;