import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ItemService } from '../core/item.service';

import { ItemDetailRouteService } from './item-detail-route.service';

describe('ItemDetailRouteService', () => {
    let service: ItemDetailRouteService;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [RouterTestingModule],
            providers: [ItemDetailRouteService, { provide: ItemService, useValue: {} }],
        });
        service = TestBed.inject(ItemDetailRouteService);
    });

    it('should be created', () => {
        expect(1).toBeTruthy();
    });
});
