import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderRoutingModule } from './order-routing.module';
import { OrderListComponent } from './order-list/order-list.component';
import { AuthorizationRule, AUTHORIZATION_RULE, TempAuthorizationRule } from '../auth/authorization-rule';
import { OrderService } from './order.service';
import { OrderDetailsComponent } from './order-details/order-details.component';

export class TempRule implements AuthorizationRule {
    authorize(): void {
        console.log('i am temp rule from order module');
    }
}

@NgModule({
  declarations: [
    OrderListComponent,
    OrderDetailsComponent
  ],
  imports: [
    CommonModule,
    OrderRoutingModule
  ],
  providers: [
    //{provide: AUTHORIZATION_RULE, useExisting: AUTHORIZATION_RULE, multi: true},
    {provide: TempAuthorizationRule, useClass: TempRule},
    OrderService
  ]
})
export class OrderModule { }
