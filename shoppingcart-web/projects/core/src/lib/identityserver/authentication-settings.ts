export interface AuthenticationSettings {
    logLevel: 'none' | 'error' | 'warn' | 'info' | 'debug';
    authority: string;
    clientId: string;
    responseType: string;
    scope: string;
    redirectUri: string;
    silentRedirectUri: string;
    postLogoutRedirectUri: string;
    automaticSilentRenew?: boolean;
    checkSessionInterval?: number;
    accessTokenExpiringNotificationTime?: number;
    filterProtocolClaims?: boolean;
}
