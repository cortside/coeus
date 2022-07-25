import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { User, UserManager, UserManagerSettings } from 'oidc-client';
import { BehaviorSubject, Observable } from 'rxjs';
import { USER_MANAGER_SETTINGS_TOKEN } from './user-manager-settings.token';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private userSubject = new BehaviorSubject<User | null>(null);
    public user: User | null = null;
    constructor(private userManager: UserManager, private router: Router) {
        this.userManager.events.addUserLoaded((u: User) => {

            this.user = u;// TODO
            console.log('addUserLoaded: ', this.user);
        });
    }

    initialize(): Promise<User | null> {
        return this.userManager.getUser().then((u) => {
            this.userSubject.next(u);
            this.user = u; // TODO
            return u;
        });
    }

    getUser(): Observable<User | null> {
        return this.userSubject.asObservable();
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
