import { TestBed } from '@angular/core/testing';
import { AuthenticationSettings } from './authentication-settings';
import { AuthenticationService } from './authentication.service';

describe('AuthenticationService', () => {
    let service: AuthenticationService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = new AuthenticationService({} as AuthenticationSettings);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
