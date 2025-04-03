import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { CatalogClient } from './catalog.client';
import { AppConfig } from 'src/environments/app-config';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

describe('CatalogClient', () => {
    let service: CatalogClient;

    beforeEach(() => {
        TestBed.configureTestingModule({
    imports: [],
    providers: [{ provide: AppConfig, useValue: {} }, provideHttpClient(withInterceptorsFromDi()), provideHttpClientTesting()]
});
        service = TestBed.inject(CatalogClient);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
