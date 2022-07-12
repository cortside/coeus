import { APP_INITIALIZER, Provider } from '@angular/core';
import { AuthenticationService } from './authentication.service';

export const authenticationIntializerProvider: Provider = {
    provide: APP_INITIALIZER,
    useFactory(service: AuthenticationService): () => Promise<void> {
        return () => service.initialize();
    },
    deps: [AuthenticationService],
    multi: true
};
