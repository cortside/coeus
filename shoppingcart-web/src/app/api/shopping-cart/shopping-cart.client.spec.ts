import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ShoppingCartClient } from './shopping-cart.client';
import { AppConfig } from 'src/environments/app-config';

describe('ShoppingCartClient', () => {
    let service: ShoppingCartClient;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [{ provide: AppConfig, useValue: {} }],
        });
        service = TestBed.inject(ShoppingCartClient);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
