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
        if (request.context.get(AUTHENTICATED_REQUEST) === true && this.auththenticationService.user) {
            console.log('authenticated request for: ' + request.url);
            request= request.clone({ setHeaders: { Authorization: `Bearer ${this.auththenticationService.user.access_token}` } });
            //request = request.clone({ setHeaders: { Authorization: `Bearer 107d225e3ea706bbbceb83593daf5c73c1347a89eaa89f253ca845beffb2e3f9` } });
        }        
         return next.handle(request);        
    }
}
