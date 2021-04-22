import React from "react";
import { Grid } from "@material-ui/core";
import Typography from "@material-ui/core/Typography";
import File from "./File";

const Files = ({ items }) => (
    items.length > 0
    ? (<Grid
        container 
        spacing={2}
        direction="row"
        justify="flex-start"
        alignItems="center">
        {items.map((item) => (
            <Grid key={item.id} item>
                <File
                    key={item.id}
                    name={item.name}
                    ext={item.ext}
                    hierarchy={item.hierarchy}
                    onClick={item.onClick}
                    actions={item.actions}
                />
            </Grid>
        ))}
    </Grid>)
    : (<Typography variant="subtitle1" gutterBottom>Файлы отсутствуют</Typography>)
);

export default Files;