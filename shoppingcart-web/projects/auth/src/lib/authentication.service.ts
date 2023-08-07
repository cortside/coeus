import { Injectable } from '@angular/core';
import { User, UserManager, UserManagerSettings } from 'oidc-client';
import { AuthenticatedUser } from './authenticated-user';
import { AuthenticationSettings } from './authentication-settings';

export class AuthenticationService {
    private readonly userManager: UserManager;

    constructor(protected settings: AuthenticationSettings) {
        this.userManager = new UserManager(<UserManagerSettings>{
            authority: settings.authority,
            client_id: settings.clientId,
            response_type: settings.responseType,
            scope: settings.scope,
            redirect_uri: settings.redirectUri,
            silent_redirect_uri: settings.silentRedirectUri,
            post_logout_redirect_uri: settings.postLogoutRedirectUri,
            automaticSilentRenew: settings.automaticSilentRenew,
            checkSessionInterval: settings.checkSessionInterval,
            accessTokenExpiringNotificationTime: settings.accessTokenExpiringNotificationTime,
            filterProtocolClaims: settings.filterProtocolClaims,
            loadUserInfo: true,
            monitorSession: true,
        });
    }

    async getUser(): Promise<AuthenticatedUser | undefined> {
        return this.userManager.getUser().then(this.mapToAuthenticatedUser);
    }

    async completeSignIn(): Promise<AuthenticatedUser | undefined> {
        if (window.location.href.indexOf(this.settings.redirectUri) > -1) {
            const redirectedUser = await this.userManager.signinRedirectCallback();
            window.history.replaceState({}, window.document.title, redirectedUser.state || '/');
            return Promise.resolve(this.mapToAuthenticatedUser(redirectedUser));
        }

        // validate user existence/renew token
        const user: User | undefined = await this.userManager.signinSilent().catch(() => undefined);
        return Promise.resolve(this.mapToAuthenticatedUser(user));
    }

    async login(): Promise<void> {
        console.log('logging in');
        return this.userManager.signinRedirect();
    }

    async isAuthenticated(): Promise<boolean> {
        return this.userManager.getUser().then((u) => u !== null && u !== undefined);
    }

    async getAuthorizationData(): Promise<string> {
        return this.userManager.getUser().then((u) => u?.access_token || '');
    }

    private mapToAuthenticatedUser(u: User | undefined | null) {
        if (u) {
            return new AuthenticatedUser(new Map<string, any>(Object.entries(u.profile)));
        }

        return undefined;
    }
}
