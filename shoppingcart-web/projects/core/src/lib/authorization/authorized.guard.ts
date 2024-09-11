import { inject } from '@angular/core';
import { AuthorizationService } from './authorization.service';

export const requireAuthorization = (policy: string): (() => Promise<boolean>) => {
    return async () => {
        const authorized = await inject(AuthorizationService).authorize(policy);
        return authorized;
    };
};
