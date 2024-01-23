import { requireAuthentication } from './authenticated.guard';

describe('AuthenticatedGuard', () => {
    beforeEach(() => {

    });

    it('should be created', () => {
        expect(requireAuthentication).toBeTruthy();
    });
});
