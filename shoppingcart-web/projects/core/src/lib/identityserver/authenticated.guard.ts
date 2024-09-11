import { inject } from '@angular/core';
import { AuthenticationService } from './authentication.service';

export const requireAuthentication = async (): Promise<boolean> => {
    const auth = inject(AuthenticationService);
    return auth.getUser().then((u) => {
        if (u) {
            return true;
        }

        return auth.login().then(() => false);
    });
};
