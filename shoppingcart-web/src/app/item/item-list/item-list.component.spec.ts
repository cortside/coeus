import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { PagedModel } from 'src/app/models/paged.model';
import { ItemCardComponent } from '../item-card/item-card.component';
import { ItemFacade } from '../item.facade';
import { ItemModel } from '../models/item.model';

import { ItemListComponent } from './item-list.component';

describe('ItemListComponent', () => {
    let component: ItemListComponent;
    let fixture: ComponentFixture<ItemListComponent>;
    let facade: jasmine.SpyObj<ItemFacade>;

    beforeEach(async () => {
        facade = jasmine.createSpyObj<ItemFacade>(ItemFacade.name, ['getItems']);
        const page = {
            totalItems: 12,
            pageNumber: 1,
            pageSize: 12,
            items: [...Array(12).keys()].map((i) => {
                return {
                    itemId: `${i}`,
                    imageUrl: `image ${i}`,
                    name: `item ${i}`,
                    sku: `sku${i}`,
                    unitPrice: i * 100,
                };
            }),
        } satisfies PagedModel<ItemModel>;
        facade.getItems.and.returnValue(of(page));

        await TestBed.configureTestingModule({
            imports: [ItemListComponent, RouterTestingModule],
            providers: [{ provide: ItemFacade, useValue: facade }],
        }).compileComponents();

        fixture = TestBed.createComponent(ItemListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
        expect(component.items()?.totalItems).toBe(12);
        expect(fixture.debugElement.query(By.css('h1')).nativeElement.textContent).toEqual('Catalog');
        expect(fixture.debugElement.queryAll(By.directive(ItemCardComponent)).length).toBe(12);
    });
});
