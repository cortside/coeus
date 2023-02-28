import { AuthenticationService, AuthorizationData, AuthorizationService } from '@muziehdesign/auth';
import { delay, switchMap, tap } from 'rxjs';
import { ShoppingCartClient } from './api/shopping-cart/shopping-cart.client';

export const initializeApplication = (
    authenticationService: AuthenticationService,
    client: ShoppingCartClient,
    authorizationService: AuthorizationService
): (() => Promise<void>) => {
    return (): Promise<void> => {
        authenticationService.onUserSignedOut().pipe(tap((x) => authorizationService.reset()));
        authenticationService.onUserSignedIn().pipe(    
            delay(1000),  //this is to prevent the authorization api call from firing before authentication user is done being set      
            switchMap((x) => client.getAuthorization()),
            tap((x) => {
                const map = new Map<string, AuthorizationData>();
                map.set(ShoppingCartClient.name, x as AuthorizationData);
            })
        ).subscribe();

        return authenticationService.initialize();
    };
};
