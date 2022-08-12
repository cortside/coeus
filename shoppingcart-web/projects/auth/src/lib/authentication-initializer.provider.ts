import { APP_INITIALIZER, Provider } from '@angular/core';
import { defer, Observable, switchMap } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { AuthorizationData } from './authorization-data';
import { AuthorizationService } from './authorization.service';

// TODO
export const createAuthInitializationProvider = (getAuthorizationData: () => Promise<Map<string, AuthorizationData>>) =>
    <Provider>{
        provide: APP_INITIALIZER,
        useFactory(authenticationService: AuthenticationService, authorizationService: AuthorizationService): () => Promise<void> {
            return async () => {
                authenticationService.onUserSignedOut().subscribe((x) => authorizationService.reset());
                authenticationService
                    .onUserSignedIn()
                    .pipe(switchMap((x) => defer(() => getAuthorizationData())))
                    .subscribe((data) => authorizationService.set(data));

                await authenticationService.initialize();
            };
        },
        deps: [AuthenticationService],
        multi: true,
    };
