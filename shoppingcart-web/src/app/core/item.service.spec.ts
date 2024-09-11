import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { ItemService } from './item.service';
import { CatalogClient } from '../api/catalog/catalog.client';
import { AuthorizationService } from '@muziehdesign/core';

describe('ItemService', () => {
    let service: ItemService;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [{ provide: CatalogClient, useValue: {} }, ItemService, { provide: AuthorizationService, useValue: {} }],
        });
        service = TestBed.inject(ItemService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
