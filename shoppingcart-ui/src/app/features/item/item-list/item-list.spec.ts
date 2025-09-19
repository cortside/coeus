import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ItemList } from './item-list';

describe('ItemList', () => {
  let component: ItemList;
  let fixture: ComponentFixture<ItemList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ItemList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ItemList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
