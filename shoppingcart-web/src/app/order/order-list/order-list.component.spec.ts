import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OrderService } from 'src/app/core/order.service';

import { OrderListComponent } from './order-list.component';

describe('OrderListComponent', () => {
    let component: OrderListComponent;
    let fixture: ComponentFixture<OrderListComponent>;

    beforeEach(async () => {
        const orderService = jasmine.createSpyObj<OrderService>(OrderService.name, ['getOrders']);        
        await TestBed.configureTestingModule({
            declarations: [OrderListComponent],
            providers: [
                { provide: OrderService, useValue: orderService }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(OrderListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
