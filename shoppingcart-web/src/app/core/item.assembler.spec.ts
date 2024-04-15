import { TestBed } from '@angular/core/testing';

import { ItemAssembler } from './item.assembler';

describe('ItemAssembler', () => {
  let service: ItemAssembler;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ItemAssembler);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
