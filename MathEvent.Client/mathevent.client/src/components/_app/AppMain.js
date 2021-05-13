import React, { useCallback, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import clsx from 'clsx';
import {
  createMuiTheme,
  ThemeProvider,
  makeStyles,
} from '@material-ui/core/styles';
import AppBar from '@material-ui/core/AppBar';
import CssBaseline from '@material-ui/core/CssBaseline';
import Divider from '@material-ui/core/Divider';
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import MenuItem from '@material-ui/core/MenuItem';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import Popover from '@material-ui/core/Popover';

import { useCurrentUser } from '../../hooks';
import { showLogoutModal } from '../../store/actions/account';
import { setGridView } from '../../store/actions/event';
import {
  navigateToEvents,
  navigateToHome,
  navigateToLogin,
  navigateToRegister,
  navigateToEventsStatistics,
  navigateToUserEdit,
} from '../../utils/navigator';
import { setIsDarkTheme } from '../../store/actions/app';
import { showUserStatistics } from '../../store/actions/user';
import { Icon, IconButton, iconTypes } from '../_common/Icon';
import { palette, paletteDark } from '../../styles/palette';
import App from './App';
import Button, { colors } from '../_common/Button';
import routes from '../../utils/routes';

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
    alignItems: 'center',
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
    display: 'flex',
    justifyContent: 'space-between',
    flexWrap: 'nowrap',
    padding: theme.spacing(0, 1),
    ...theme.mixins.toolbar,
  },
  toolbarIcon: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
    padding: '0 8px',
    ...theme.mixins.toolbar,
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
  },
}));

const AppMain = () => {
  const dispatch = useDispatch();
  const { isDarkTheme } = useSelector((state) => state.app);
  const { isGridView } = useSelector((state) => state.event);
  const currentRoute = useSelector((state) => state.router.location.pathname);
  const { userInfo, isAuthenticated, isFetching } = useCurrentUser();

  const [anchorEl, setAnchorEl] = useState(null);
  const [open, setOpen] = useState(false);

  const theme = createMuiTheme({
    palette: isDarkTheme ? paletteDark : palette,
  });
  const classes = useStyles(theme);

  const handleLightTheme = () => {
    dispatch(setIsDarkTheme(false));
  };

  const handleDarkTheme = () => {
    dispatch(setIsDarkTheme(true));
  };

  const handleListView = () => {
    dispatch(setGridView(false));
  };

  const handleGridView = () => {
    dispatch(setGridView(true));
  };

  const handleLoginClick = () => navigateToLogin();
  const handleRegisterClick = () => navigateToRegister();
  const handleLogoutClick = () => {
    setAnchorEl(null);
    dispatch(showLogoutModal());
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

  const handleMenuSettings = useCallback(() => {
    if (userInfo.sub) {
      navigateToUserEdit(userInfo.sub);
    }

    setAnchorEl(null);
  }, [dispatch, userInfo]);

  const handleMenuStatistics = useCallback(() => {
    if (userInfo) {
      dispatch(showUserStatistics({ user: userInfo }));
    }

    setAnchorEl(null);
  }, [dispatch, userInfo]);

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
              {!isAuthenticated ? (
                <>
                  <Button
                    className={classes.menuButton}
                    onClick={handleRegisterClick}
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
              ) : isFetching ? (
                <div>Ожидайте...</div>
              ) : (
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
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                    transformOrigin={{ vertical: 'top', horizontal: 'right' }}
                  >
                    <MenuItem onClick={handleMenuClose}>
                      {userInfo.email} {userInfo.name}
                    </MenuItem>
                    <Divider />
                    <MenuItem onClick={handleMenuStatistics}>
                      <ListItemIcon>
                        <Icon type={iconTypes.stats} />
                      </ListItemIcon>
                      Статистика
                    </MenuItem>
                    <MenuItem onClick={handleMenuSettings}>
                      <ListItemIcon>
                        <Icon type={iconTypes.person} />
                      </ListItemIcon>
                      Кабинет
                    </MenuItem>
                    <MenuItem onClick={handleLogoutClick}>
                      <ListItemIcon>
                        <Icon type={iconTypes.logout} />
                      </ListItemIcon>
                      Выйти
                    </MenuItem>
                  </Popover>
                </>
              )}
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
            <IconButton type={iconTypes.left} onClick={handleDrawerClose} />
          </div>
          <Divider />
          <List>
            <ListItem
              button
              key="Главная"
              selected={isSelected(routes.home)}
              onClick={navigateToHome}
            >
              <ListItemIcon>
                <Icon type={iconTypes.home} />
              </ListItemIcon>
              <ListItemText primary="Главная" />
            </ListItem>
            <ListItem
              button
              key="События"
              selected={isSelected(routes.events.main)}
              onClick={navigateToEvents}
            >
              <ListItemIcon>
                <Icon type={iconTypes.events} />
              </ListItemIcon>
              <ListItemText primary="События" />
            </ListItem>
            <ListItem
              button
              key="Статистика"
              selected={isSelected(routes.statistics.main)}
              onClick={navigateToEventsStatistics}
            >
              <ListItemIcon>
                <Icon type={iconTypes.stats} />
              </ListItemIcon>
              <ListItemText primary="Статистика" />
            </ListItem>
            {isAuthenticated ? (
              <ListItem button key="Выйти" onClick={handleLogoutClick}>
                <ListItemIcon>
                  <Icon type={iconTypes.exit} />
                </ListItemIcon>
                <ListItemText primary="Выйти" />
              </ListItem>
            ) : (
              <ListItem button key="Войти" onClick={handleLoginClick}>
                <ListItemIcon>
                  <Icon type={iconTypes.login} />
                </ListItemIcon>
                <ListItemText primary="Войти" />
              </ListItem>
            )}
          </List>
          <Divider />
          {isDarkTheme ? (
            <ListItem button key="Светлая" onClick={handleLightTheme}>
              <ListItemIcon>
                <Icon type={iconTypes.setLight} />
              </ListItemIcon>
              <ListItemText primary="Светлая" />
            </ListItem>
          ) : (
            <ListItem button key="Темная" onClick={handleDarkTheme}>
              <ListItemIcon>
                <Icon type={iconTypes.setDark} />
              </ListItemIcon>
              <ListItemText primary="Темная" />
            </ListItem>
          )}
          {isGridView ? (
            <ListItem button key="Список" onClick={handleListView}>
              <ListItemIcon>
                <Icon type={iconTypes.list} />
              </ListItemIcon>
              <ListItemText primary="Вид: список" />
            </ListItem>
          ) : (
            <ListItem button key="Карточки" onClick={handleGridView}>
              <ListItemIcon>
                <Icon type={iconTypes.dashboard} />
              </ListItemIcon>
              <ListItemText primary="Вид: карточки" />
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
};

export default AppMain;
