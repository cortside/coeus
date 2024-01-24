import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { NgFormModelState, NgFormModelStateFactory } from '@muziehdesign/forms';
import { of } from 'rxjs';
import { ItemService } from 'src/app/core/item.service';
import { ShoppingCartService } from 'src/app/core/shopping-cart.service';
import { ItemModel } from '../models/item.model';

import { ItemComponent } from './item.component';

describe('ItemComponent', () => {
    let component: ItemComponent;
    let fixture: ComponentFixture<ItemComponent>;
    const sku = 'ITEMSKU-12';

    beforeEach(async () => {
        const itemService = jasmine.createSpyObj<ItemService>(ItemService.name, { getItem: of(), getItems: of() });
        const stateFactory = jasmine.createSpyObj<NgFormModelStateFactory>(NgFormModelStateFactory.name, ['create']);
        const cartService = jasmine.createSpyObj<ShoppingCartService>(ShoppingCartService.name, ['addItem']);

        await TestBed.configureTestingModule({
            imports: [ItemComponent],
            providers: [
                { provide: ActivatedRoute, useValue: { snapshot: { params: sku } } },
                { provide: ItemService, useValue: itemService },
                { provide: NgFormModelStateFactory, useValue: stateFactory },
                { provide: ShoppingCartService, useValue: cartService }
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(ItemComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
