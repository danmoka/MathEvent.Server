import settings from "./config.json";

// TODO: client secret
const config = {
    baseUrl: settings.baseUrl,
    siteUrl: settings.siteUrl,
    accountUrl: settings.accountUrl,
    clientId: settings.clientId,
    clientSecret: settings.clientSecret,
    passwordGrantType: settings.passwordGrantType,
    refreshGrantType: settings.refreshGrantType,
};

export default config;