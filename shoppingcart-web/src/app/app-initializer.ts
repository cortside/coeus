import { AuthenticationService, AuthorizationData, AuthorizationService } from '@muziehdesign/auth';
import { firstValueFrom } from 'rxjs';
import { ShoppingCartClient } from './api/shopping-cart/shopping-cart.client';

export const initializeApplication = (authenticationService: AuthenticationService, client: ShoppingCartClient, service: AuthorizationService): (() => Promise<void>) => {
    return (): Promise<void> => {
        return authenticationService.initialize().then((u) => {
            if(!u) {
                return;
            }
            return firstValueFrom(client.getAuthorization()).then((x) => service.register('ShoppingCart', x as AuthorizationData));
        });
    };
};
