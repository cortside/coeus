import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpContextToken } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';

export const AUTHENTICATED_REQUEST = new HttpContextToken<boolean>(() => false);

@Injectable()
export class AuthenticationTokenInterceptor implements HttpInterceptor {
    constructor(private auththenticationService: AuthenticationService) {}

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        const c = request.context.get(AUTHENTICATED_REQUEST);
        if (request.context.get(AUTHENTICATED_REQUEST) === true) {
            request= request.clone({ setHeaders: { Authorization: `${this.auththenticationService.getAuthorization()}` }});
        }        
         return next.handle(request);        
    }
}
