import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpContextToken } from '@angular/common/http';
import { from, map, Observable, switchMap } from 'rxjs';
import { AuthenticationService } from './authentication.service';

export const AUTHENTICATED_REQUEST = new HttpContextToken<boolean>(() => false);

@Injectable()
export class AuthenticationTokenInterceptor implements HttpInterceptor {
    constructor(private auththenticationService: AuthenticationService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const c = request.context.get(AUTHENTICATED_REQUEST);
        if (request.context.get(AUTHENTICATED_REQUEST) === true) {
            return from(this.auththenticationService.getAuthorizationData()).pipe(
                switchMap((x) => {
                    request = request.clone({ setHeaders: { Authorization: x } });
                    return next.handle(request);
                })
            );
        }
        return next.handle(request);
    }
}
