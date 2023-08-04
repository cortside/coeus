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

        console.log(settings);
    }

    async getUser(): Promise<AuthenticatedUser | undefined> {
        return this.userManager.getUser().then(this.mapToAuthenticatedUser);
    }

    async bootstrap(): Promise<AuthenticatedUser | undefined> {
        // intercept silent redirect and halt actual bootstrap
        if (this.userManager.settings.silent_redirect_uri != null && window.location.href.indexOf(this.userManager.settings.silent_redirect_uri) > -1) {
            return this.userManager.signinSilentCallback().then(this.mapToAuthenticatedUser);
        }

        // handle signin callback
        if (this.userManager.settings.redirect_uri != null && window.location.href.indexOf(this.userManager.settings.redirect_uri) > -1) {
            const redirectedUser = await this.userManager.signinRedirectCallback();
            window.history.replaceState({}, window.document.title, redirectedUser.state);
            return this.mapToAuthenticatedUser(redirectedUser);
        }

        // validate user existence/renew token
        const user: User | undefined = await this.userManager.signinSilent().catch(() => undefined);
        return Promise.resolve(this.mapToAuthenticatedUser(user));
    }

    async login(): Promise<void> {
        return this.userManager.signinRedirect();
    }

    async isAuthenticated(): Promise<boolean> {
        return this.userManager.getUser().then((u) => {
            if (u) {
                return true;
            }

            return false;
        });
    }

    async getAuthorizationData(): Promise<string> {
        return this.userManager.getUser().then(u=>u?.access_token || '');
    }

    private mapToAuthenticatedUser(u: User | undefined | null) {
        console.log('mapping', u);
        if(u) {
            return new AuthenticatedUser(new Map<string, any>(Object.entries(u.profile)));
        }

        return undefined;
    }
    /*
    private userSubject = new BehaviorSubject<User | null>(null);
    public user: User | null = null;
    constructor(private userManager: UserManager, private router: Router) {
        this.userManager.events.addUserLoaded((u: User) => {
            this.user = u; // TODO
            console.log('addUserLoaded: ', this.user);
        });
    }

    initialize(): Promise<void> {
        return this.userManager.getUser().then((u) => {
            this.userSubject.next(u);
            this.user = u; // TODO
        });
    }

    getUser(): Observable<User | null> {
        return this.userSubject.asObservable();
        //return this.userManager.getUser()
    }

    redirectToSignIn(): Promise<void> {
        console.log('redirect to sign in from url', this.router.url);
        return this.userManager.signinRedirect({ state: this.router.url });
    }

    onUserSignedIn(): Observable<User> {
        return this.userSubject.pipe(
            filter((u) => u != null),
            map((u) => u as User)
        );
    }

    onUserSignedOut(): Observable<void> {
        return this.userSubject.pipe(
            filter((u) => !u),
            map((u) => undefined)
        );
    }*/
}
