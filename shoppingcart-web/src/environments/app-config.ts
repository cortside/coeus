import { AuthenticationSettings } from "@muziehdesign/auth";

export class AppConfig {
    application?: {
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
