import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { User, UserManager, UserManagerSettings } from 'oidc-client';
import { USER_MANAGER_SETTINGS_TOKEN } from './user-manager-settings.token';

@Injectable({
    providedIn: 'root',
})
export class AuthenticationService {
    private userManager: UserManager;
    public user: User | undefined;
    constructor(
        @Inject(USER_MANAGER_SETTINGS_TOKEN)
        userManagerSettings: UserManagerSettings,
        private router: Router
    ) {
    const defaults = <UserManagerSettings>{
            checkSessionInterval: 10000,
            monitorSession: true,
            automaticSilentRenew: true,
            response_type: 'id_token token',
        };
        this.userManager = new UserManager(Object.assign({}, defaults, userManagerSettings));
        this.userManager.events.addUserLoaded((u: User) => {
            this.user = u;
            console.log('addUserLoaded: ', this.user);
        });
    }

    async initialize(): Promise<void> {
        console.log('initializing');
        const url = window.location.href;
        console.log(`handling login with url ${url}, ${window.location.href}`);
        console.log(this.userManager.settings.redirect_uri + ' ' + this.userManager.settings.silent_redirect_uri);

        if (this.userManager.settings.silent_redirect_uri && url.indexOf(this.userManager.settings.silent_redirect_uri) > -1) {
            console.log('silent redirect, skip');
            return this.userManager.signinSilentCallback().then(x=>{
                this.user = x;
                console.log(this.user?.profile.name);
                return Promise.resolve();
            });
        }

        if (this.userManager.settings.redirect_uri && url.indexOf(this.userManager.settings.redirect_uri) > -1) {
            console.log('redirect url');
            const redirectedUser = await this.userManager.signinRedirectCallback();
            console.log('replacing with state', redirectedUser.state);
            window.history.replaceState({}, window.document.title, redirectedUser.state);
            return Promise.resolve();
        }

        console.log('getting current user');
        return this.userManager
            .signinSilent()
            .then((u) => {
                console.log('silent signin completed', u);
                return Promise.resolve();
            }).catch(()=>{
                return this.userManager.signinRedirect({ state: window.location.href });
            });
    }

    isAuthenticated(): boolean {
        return this.user !== undefined;
    }

    getSignInRedirect(): Promise<string> {
        return this.userManager.signinRedirectCallback().then((redirectedUser) => {
            return redirectedUser?.state || '';
        });
    }

    handleSignInSilentRedirect(): Promise<User | undefined> {
        return this.userManager.signinSilentCallback();
    }

    redirectToSignIn(): Promise<void> {
        console.log('redirect to sign in from url', this.router.url);
        return this.userManager.signinRedirect({ state: this.router.url });
    }
}
