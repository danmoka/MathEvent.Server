import React, { useState } from 'react';
import { useSelector, useDispatch } from "react-redux";
import clsx from 'clsx';
import { createMuiTheme, ThemeProvider, makeStyles } from '@material-ui/core/styles';
import AppBar from '@material-ui/core/AppBar';
import CssBaseline from '@material-ui/core/CssBaseline';
import Divider from '@material-ui/core/Divider';
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import MenuItem from "@material-ui/core/MenuItem";
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import Popover from "@material-ui/core/Popover";

import { useCurrentUser } from "../../hooks";
import { logout, revocation } from "../../store/actions/account";
import { navigateToEvents, navigateToHome, navigateToLogin } from "../../utils/navigator";
import { setIsDarkTheme } from "../../store/actions/app";
import { Icon, IconButton, iconTypes } from "../_common/Icon";
import { palette, paletteDark } from "../../styles/palette";
import App from './App';
import Button, { colors } from "../_common/Button";
import routes from "../../utils/routes";

const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
  },
  appBar: {
    display: 'flex',
    justifyContent: 'space-between',
    flexWrap: 'nowrap',
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
  },
  appBarShift: {
    marginLeft: drawerWidth,
    width: `calc(100% - ${drawerWidth}px)`,
    transition: theme.transitions.create(['width', 'margin'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  section: {
    display: 'flex',
    alignItems: 'center'
  },
  menuButton: {
    boxShadow: 'none',
    borderRadius: '0',
    textTransform: 'none',
  },
  hide: {
    display: 'none',
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
    whiteSpace: 'nowrap',
  },
  drawerOpen: {
    width: drawerWidth,
    transition: theme.transitions.create('width', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  drawerClose: {
    transition: theme.transitions.create('width', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    overflowX: 'hidden',
    width: theme.spacing(7) + 1,
    [theme.breakpoints.up('sm')]: {
      width: theme.spacing(7) + 1,
    },
  },
  toolbar: {
    display: "flex",
    justifyContent: "space-between",
    flexWrap: "nowrap",
    padding: theme.spacing(0, 1),
    ...theme.mixins.toolbar,
  },
  toolbarIcon: {
    display: "flex",
    alignItems: "center",
    justifyContent: "flex-end",
    padding: "0 8px",
    ...theme.mixins.toolbar
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
  },
}));

const AppMain = () => {
  const dispatch = useDispatch();
  const { isDarkTheme } = useSelector(state => state.app);
  const currentRoute = useSelector(state => state.router.location.pathname);
  const { userInfo, isAuthenticated, isFetching } = useCurrentUser();

  const [anchorEl, setAnchorEl] = useState(null);
  const [open, setOpen] = useState(false);

  const theme = createMuiTheme({ palette: isDarkTheme ? paletteDark : palette });
  const classes = useStyles();

  const handleLightTheme = () => {
    dispatch(setIsDarkTheme(false));
  };

  const handleDarkTheme = () => {
    dispatch(setIsDarkTheme(true));
  };

  const handleLoginClick = () => navigateToLogin();
  const handleLogoutClick = () => {
    dispatch(logout());
    dispatch(revocation());
    handleMenuClose();
  };

  const handleDrawerOpen = () => {
    setOpen(true);
  };

  const handleDrawerClose = () => {
    setOpen(false);
  };

  const isSelected = (route) => currentRoute.includes(route);

  const handleMenuOpen = (event) => setAnchorEl(event.currentTarget);
  const handleMenuClose = () => setAnchorEl(null);

  return (
    <ThemeProvider theme={theme}>
      <div className={classes.root}>
        <CssBaseline />
        <AppBar
          position="fixed"
          className={clsx(classes.appBar, {
            [classes.appBarShift]: open,
          })}
        >
          <Toolbar className={classes.toolbar}>
            <section className={classes.section}>
              <IconButton
                color={colors.inherit}
                type={iconTypes.menu}
                onClick={handleDrawerOpen}
              />
              <Typography variant="h6" noWrap>
                MathEvent
              </Typography>
            </section>
            <section className={classes.section}>
              {!isAuthenticated
                ? (
                  <>
                    <Button
                      className={classes.menuButton}
                      onClick={() => { console.log("register button clicked"); }}
                    >
                      Регистрация
                    </Button>
                    <Button
                      className={classes.menuButton}
                      onClick={handleLoginClick}
                    >
                      Войти
                    </Button>
                  </>
                ) : (
                  isFetching
                    ? (<div>Ожидайте...</div>)
                    : (
                      <>
                        <Button
                          className={classes.menuButton}
                          color={colors.transparentBlack}
                          endIcon={iconTypes.account}
                          onClick={handleMenuOpen}
                        >
                          {userInfo.email}
                        </Button>
                        <Popover
                          id="app-bar-popover"
                          open={Boolean(anchorEl)}
                          anchorEl={anchorEl}
                          onClose={handleMenuClose}
                          anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
                          transformOrigin={{ vertical: "top", horizontal: "right" }}
                        >
                          <MenuItem onClick={handleMenuClose}>
                            {userInfo.email} {userInfo.name}
                          </MenuItem>
                          <Divider />
                          <MenuItem onClick={handleMenuClose}>
                            <ListItemIcon>
                              <Icon type={iconTypes.stats} />
                            </ListItemIcon>
                              Статистика
                            </MenuItem>
                          <MenuItem onClick={handleMenuClose}>
                            <ListItemIcon>
                              <Icon type={iconTypes.settings} />
                            </ListItemIcon>
                              Настройки
                          </MenuItem>
                          <MenuItem onClick={handleLogoutClick}>
                            <ListItemIcon>
                              <Icon type={iconTypes.logout} />
                            </ListItemIcon>
                              Выйти
                          </MenuItem>
                        </Popover>
                      </>))}
            </section>
          </Toolbar>
        </AppBar>
        <Drawer
          variant="permanent"
          className={clsx(classes.drawer, {
            [classes.drawerOpen]: open,
            [classes.drawerClose]: !open,
          })}
          classes={{
            paper: clsx({
              [classes.drawerOpen]: open,
              [classes.drawerClose]: !open,
            }),
          }}
        >
          <div className={classes.toolbarIcon}>
            <IconButton
              type={iconTypes.left}
              onClick={handleDrawerClose} />
          </div>
          <Divider />
          <List>
            <ListItem
              button
              key="Главная"
              selected={isSelected(routes.home)}
              onClick={navigateToHome}>
              <ListItemIcon>
                <Icon
                  type={iconTypes.home}
                />
              </ListItemIcon>
              <ListItemText primary="Главная" />
            </ListItem>
            <ListItem
              button
              key="События"
              selected={isSelected(routes.events)}
              onClick={navigateToEvents}>
              <ListItemIcon>
                <Icon
                  type={iconTypes.events}
                />
              </ListItemIcon>
              <ListItemText primary="События" />
            </ListItem>
          </List>
          <Divider />
          {isDarkTheme
            ? (
              <ListItem
                button
                key="Светлая"
                onClick={handleLightTheme}>
                <ListItemIcon>
                  <Icon
                    type={iconTypes.setLight}
                  />
                </ListItemIcon>
                <ListItemText primary="Светлая" />
              </ListItem>
            )
            : (
              <ListItem
                button
                key="Темная"
                onClick={handleDarkTheme}>
                <ListItemIcon>
                  <Icon
                    type={iconTypes.setDark}
                  />
                </ListItemIcon>
                <ListItemText primary="Темная" />
              </ListItem>
            )}
        </Drawer>
        <main className={classes.content}>
          <div className={classes.toolbar} />
          <App />
        </main>
      </div>
    </ThemeProvider>
  );
}

export default AppMain;