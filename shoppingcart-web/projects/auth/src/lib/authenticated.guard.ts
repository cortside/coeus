import { inject } from '@angular/core';
import { CanMatchFn } from '@angular/router';
import { Route, UrlSegment } from '@angular/router';
import { AuthenticationService } from './authentication.service';

export const requireAuthentication: CanMatchFn = async (route: Route, segments: UrlSegment[]): Promise<boolean> => {
    const auth = inject(AuthenticationService);
    return auth.getUser().then((u) => {
        if (u) {
            return true;
        }

        return auth.login().then(() => false);
    });
};
