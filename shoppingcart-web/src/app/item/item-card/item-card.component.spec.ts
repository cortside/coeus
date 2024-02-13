import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ItemCardComponent } from './item-card.component';

describe('ItemCardComponent', () => {
    let component: ItemCardComponent;
    let fixture: ComponentFixture<ItemCardComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ItemCardComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(ItemCardComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    xit('should create', () => {
        expect(component).toBeTruthy();
    });
});
