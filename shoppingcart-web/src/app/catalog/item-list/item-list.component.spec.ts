import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Observable, of } from 'rxjs';
import { CatalogClient } from 'src/app/api/catalog/catalog.client';

import { ItemListComponent } from './item-list.component';

describe('ItemListComponent', () => {
    let component: ItemListComponent;
    let fixture: ComponentFixture<ItemListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ItemListComponent],
            providers: [{ provide: CatalogClient, useValue: { getItems: (): Observable<null> => of(null) } }],
        }).compileComponents();

        fixture = TestBed.createComponent(ItemListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
