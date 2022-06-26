import { TestBed } from '@angular/core/testing';

import { CatalogClient } from './catalog.client';

describe('CatalogClient', () => {
  let service: CatalogClient;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CatalogClient);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
