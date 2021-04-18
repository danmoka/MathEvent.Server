import React from "react";
import { Grid } from "@material-ui/core";
import File from "./File";

const Files = ({ items }) => (
    items.length > 0
    ? (<Grid
        container 
        spacing={2}
        direction="row"
        justifyContent="flex-start"
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
    : (<p>Файлы отсутствуют</p>)
);

export default Files;