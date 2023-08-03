import { AuthenticationSettings } from "@muziehdesign/auth";

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
    identity?: AuthenticationSettings;
}
