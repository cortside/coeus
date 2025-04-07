import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthenticationService } from '@muziehdesign/core';
import { OrderService } from '../core/order.service';

import { CartComponent } from './cart.component';

describe('CartComponent', () => {
  let component: CartComponent;
  let fixture: ComponentFixture<CartComponent>;

  beforeEach(() => {
    const orderService = jasmine.createSpyObj<OrderService>(OrderService.name, ['createOrder']);
    const authenticationService = jasmine.createSpyObj<AuthenticationService>(AuthenticationService.name, ['login']);
    TestBed.configureTestingModule({
      imports: [CartComponent, RouterTestingModule],
      providers: [
        {provide: OrderService, useValue: orderService},
        {provide: AuthenticationService, useValue: authenticationService}
      ]
    });
    fixture = TestBed.createComponent(CartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
