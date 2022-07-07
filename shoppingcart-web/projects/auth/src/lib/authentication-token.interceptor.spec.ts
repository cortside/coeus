import { TestBed } from '@angular/core/testing';

import { AuthenticationTokenInterceptor } from './authentication-token.interceptor';

describe('AuthenticationTokenInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      AuthenticationTokenInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: AuthenticationTokenInterceptor = TestBed.inject(AuthenticationTokenInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
