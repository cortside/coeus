import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CatalogClient } from './catalog.client';
import { AppConfig } from 'src/environments/app-config';

describe('CatalogClient', () => {
    let service: CatalogClient;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [{ provide: AppConfig, useValue: {} }],
        });
        service = TestBed.inject(CatalogClient);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
