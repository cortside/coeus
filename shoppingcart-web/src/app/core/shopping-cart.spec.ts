import { TestBed } from '@angular/core/testing';

import { ShoppingCart } from './shopping-cart';

describe('ShoppingCart', () => {
    let service: ShoppingCart;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(ShoppingCart);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
