import { User, UserManager, UserManagerSettings } from 'oidc-client';
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

    async getUser(): Promise<User | undefined> {
        return this.userManager.getUser().then((u) => u || undefined);
    }

    async bootstrap(): Promise<User | undefined> {
        // intercept silent redirect and halt actual bootstrap
        if (this.userManager.settings.silent_redirect_uri != null && window.location.href.indexOf(this.userManager.settings.silent_redirect_uri) > -1) {
            return this.userManager.signinSilentCallback();
        }

        // handle signin callback
        if (this.userManager.settings.redirect_uri != null && window.location.href.indexOf(this.userManager.settings.redirect_uri) > -1) {
            const redirectedUser = await this.userManager.signinRedirectCallback();
            window.history.replaceState({}, window.document.title, redirectedUser.state);
            return redirectedUser;
        }

        // validate user existence/renew token
        const user: User | undefined = await this.userManager.signinSilent().catch(() => undefined);
        return Promise.resolve(user);
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

    async getAuthorization(): Promise<string> {
        return this.userManager.getUser().then(u=>u?.access_token || '');
    }

    async getAuthorizationData(): Promise<string> {
        return this.userManager.getUser().then((u) => `Bearer ${u?.access_token || ''}`);
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

    isAuthenticated(): boolean {
        console.log('authenticated', this.user);
        return this.user != null;
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
