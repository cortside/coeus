import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { User, UserManager, UserManagerSettings } from 'oidc-client';
import { USER_MANAGER_SETTINGS_TOKEN } from './user-manager-settings.token';

@Injectable({providedIn: 'root'})
export class AuthenticationService {
    public user: User | null = null;
    constructor(private userManager: UserManager, private router: Router) {
        this.userManager.events.addUserLoaded((u: User) => {
            this.user = u;
            console.log('addUserLoaded: ', this.user);
        });
    }

    initialize(): Promise<void> {
        return this.userManager.getUser().then((u) => {
            this.user = u;
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
