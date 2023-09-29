import { TestBed } from '@angular/core/testing';

import { AuthenticationService } from './authentication.service';

describe('AuthenticationService', () => {
    let service: AuthenticationService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = new AuthenticationService({});
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
