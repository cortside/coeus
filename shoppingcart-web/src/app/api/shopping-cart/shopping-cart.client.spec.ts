import { TestBed } from '@angular/core/testing';

import { ShoppingCartClient } from './shopping-cart.client';

describe('ShoppingCartClient', () => {
  let service: ShoppingCartClient;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ShoppingCartClient);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
