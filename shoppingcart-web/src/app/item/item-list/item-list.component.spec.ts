import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Observable, of } from 'rxjs';
import { ItemFacade } from '../item.facade';

import { ItemListComponent } from './item-list.component';

describe('ItemListComponent', () => {
    let component: ItemListComponent;
    let fixture: ComponentFixture<ItemListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ItemListComponent],
            providers: [{ provide: ItemFacade, useValue: { getItems: (): Observable<null> => of(null) } }],
        }).compileComponents();

        fixture = TestBed.createComponent(ItemListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
