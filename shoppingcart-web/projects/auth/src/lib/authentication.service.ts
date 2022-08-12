import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { User, UserManager, UserManagerSettings } from 'oidc-client';
import { BehaviorSubject, filter, map, Observable } from 'rxjs';
import { USER_MANAGER_SETTINGS_TOKEN } from './user-manager-settings.token';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
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
    }
}
