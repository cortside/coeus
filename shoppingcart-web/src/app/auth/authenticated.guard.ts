import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanLoad, Route, RouterStateSnapshot, UrlSegment, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';

@Injectable({
    providedIn: 'root',
})
export class AuthenticatedGuard implements CanLoad, CanActivate {
    constructor(private service: AuthenticationService) {}
    canLoad(route: Route, segments: UrlSegment[]): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        console.log('can load');
        if (!this.service.isAuthenticated()) {
            return this.service.redirectToSignIn().then((x) => false);
        }

        return true;
    }
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
        console.log('can activate');
        if (!this.service.isAuthenticated()) {
            return this.service.redirectToSignIn().then((x) => false);
        }

        return true;
    }
}
