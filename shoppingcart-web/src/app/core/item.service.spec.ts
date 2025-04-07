import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { ItemService } from './item.service';
import { CatalogClient } from '../api/catalog/catalog.client';
import { AuthorizationService } from '@muziehdesign/core';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

describe('ItemService', () => {
    let service: ItemService;

    beforeEach(() => {
        TestBed.configureTestingModule({
    imports: [],
    providers: [{ provide: CatalogClient, useValue: {} }, ItemService, { provide: AuthorizationService, useValue: {} }, provideHttpClient(withInterceptorsFromDi()), provideHttpClientTesting()]
});
        service = TestBed.inject(ItemService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
