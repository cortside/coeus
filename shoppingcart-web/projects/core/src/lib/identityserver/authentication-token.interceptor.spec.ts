import { TestBed } from '@angular/core/testing';

import { AuthenticationTokenInterceptor } from './authentication-token.interceptor';
import { AuthenticationService } from './authentication.service';

describe('AuthenticationTokenInterceptor', () => {
    beforeEach(() => {
        let authenticationService = jasmine.createSpyObj(AuthenticationService.name, ['getUser', 'interceptSilentRedirect']);
        TestBed.configureTestingModule({
            providers: [AuthenticationTokenInterceptor, { provide: AuthenticationService, useValue: authenticationService }],
        });
    });

    it('should be created', () => {
        const interceptor: AuthenticationTokenInterceptor = TestBed.inject(AuthenticationTokenInterceptor);
        expect(interceptor).toBeTruthy();
    });
});
