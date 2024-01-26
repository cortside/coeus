import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { ItemService } from './item.service';
import { CatalogClient } from '../api/catalog/catalog.client';
import { ShoppingCart } from '../core/shopping-cart';

describe('ItemService', () => {
    let service: ItemService;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [
                { provide: CatalogClient, useValue: {} },
                { provide: ShoppingCart, useValue: {} },
                ItemService
            ],
        });
        service = TestBed.inject(ItemService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
