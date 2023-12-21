import { TestBed } from '@angular/core/testing';
import { AUTHORIZATION_POLICY } from './authorization-policy';

import { AuthorizationService } from './authorization.service';

describe('AuthorizationService', () => {
    let service: AuthorizationService;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                {provide: AUTHORIZATION_POLICY, useValue: []}
            ]
        });
        service = TestBed.inject(AuthorizationService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
