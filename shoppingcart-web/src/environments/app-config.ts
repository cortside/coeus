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
    identity?: {
        authority: string;
        client_id: string;
        redirect_uri: string;
        silent_redirect_uri: string;
        scope: string;
    }
}
