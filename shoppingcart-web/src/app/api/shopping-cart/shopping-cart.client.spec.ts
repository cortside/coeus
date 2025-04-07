import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ShoppingCartClient } from './shopping-cart.client';
import { AppConfig } from 'src/environments/app-config';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

describe('ShoppingCartClient', () => {
    let service: ShoppingCartClient;

    beforeEach(() => {
        TestBed.configureTestingModule({
    imports: [],
    providers: [{ provide: AppConfig, useValue: {} }, provideHttpClient(withInterceptorsFromDi()), provideHttpClientTesting()]
});
        service = TestBed.inject(ShoppingCartClient);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
