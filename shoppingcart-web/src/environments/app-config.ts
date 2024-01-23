import { AuthenticationSettings } from '@muziehdesign/core';
import { BuildInfo } from './build-info';

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
    build: BuildInfo = {} as BuildInfo;
}
