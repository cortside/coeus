import { AuthenticationSettings } from '@muziehdesign/core';

export class AppConfig {
    service?: {
        name: string;
    };
    catalogApi?: {
        url: string;
    };
    shoppingCartApi?: {
        url: string;
    };
    identity: AuthenticationSettings = {} as AuthenticationSettings;
}
