import { inject } from '@angular/core';
import { CanMatchFn } from '@angular/router';
import { Route, UrlSegment } from '@angular/router';
import { AuthenticationService } from './authentication.service';

export const canMatchIfAuthenticated: CanMatchFn = (route: Route, segments: UrlSegment[]) => {
    const auth = inject(AuthenticationService);
    if (!auth.isAuthenticated()) {
        return auth.login().then(() => false);
    }

    return true;
};
