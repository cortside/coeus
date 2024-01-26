import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { NgFormModelStateFactory } from '@muziehdesign/forms';
import { of } from 'rxjs';
import { ItemService } from '../item.service';
import { ItemDetailComponent } from './item-detail.component';

describe('ItemDetailComponent', () => {
  let component: ItemDetailComponent;
  let fixture: ComponentFixture<ItemDetailComponent>;
  const sku = 'ITEMSKU-12';

  beforeEach(async () => {
      const itemService = jasmine.createSpyObj<ItemService>(ItemService.name, { getItem: of(), getItems: of() });
      const stateFactory = jasmine.createSpyObj<NgFormModelStateFactory>(NgFormModelStateFactory.name, ['create']);

      await TestBed.configureTestingModule({
          imports: [ItemDetailComponent],
          providers: [
              { provide: ActivatedRoute, useValue: { snapshot: { params: sku } } },
              { provide: ItemService, useValue: itemService },
              { provide: NgFormModelStateFactory, useValue: stateFactory }
          ],
      }).compileComponents();

      fixture = TestBed.createComponent(ItemDetailComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
  });

  it('should create', () => {
      expect(component).toBeTruthy();
  });
});