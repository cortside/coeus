import { Log, User, UserManager, UserManagerSettings } from 'oidc-client';
import { AuthenticatedUser } from './authenticated-user';
import { AuthenticationSettings } from './authentication-settings';

export class AuthenticationService {
    private readonly userManager: UserManager;

    constructor(protected settings: AuthenticationSettings) {
        Log.logger = console; // TODO
        const map = new Map<string, number>();
        map.set('debug', Log.DEBUG);
        map.set('error', Log.ERROR);
        map.set('none', Log.NONE);
        map.set('warn', Log.WARN);
        map.set('info', Log.INFO);
        Log.level = map.get(settings.logLevel) || Log.NONE;

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
        this.userManager.events.addUserSignedOut(async () => {
            await this.userManager.signoutRedirect();
        });
    }

    async getUser(): Promise<AuthenticatedUser | undefined> {
        return this.userManager.getUser().then((x) => {
            return this.mapToAuthenticatedUser(x);
        });
    }

    async interceptSilentRedirect(): Promise<boolean> {
        // intercept silent redirect and halt actual bootstrap
        if (window.location.href.indexOf(this.settings.silentRedirectUri) > -1) {
            await this.userManager.signinSilentCallback();
            return true;
        }

        return false;
    }

    async completeSignIn(): Promise<AuthenticatedUser | undefined> {
        // handle signin callback
        if (window.location.href.indexOf(this.settings.redirectUri) > -1) {
            const redirectedUser = await this.userManager.signinRedirectCallback();
            window.history.replaceState({}, window.document.title, redirectedUser.state || '/');
        }

        // validate user existence/renew token
        const user: User | null | undefined = await this.userManager.signinSilent().catch(() => undefined);
        return Promise.resolve(this.mapToAuthenticatedUser(user));
    }

    async login(): Promise<void> {
        return this.userManager.signinRedirect();
    }

    async isAuthenticated(): Promise<boolean> {
        return this.userManager.getUser().then((u) => u !== null && u !== undefined);
    }

    async getAuthorizationData(): Promise<string> {
        return this.userManager.getUser().then((u) => {
            if (u?.access_token) {
                return `Bearer ${u.access_token}`;
            }
            
            return '';
        });
    }

    private mapToAuthenticatedUser(user: User | undefined | null) {
        if (user) {
            return new AuthenticatedUser(new Map<string, any>(Object.entries(user.profile)));
        }

        return undefined;
    }
}
