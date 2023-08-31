import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderRoutingModule } from './order-routing.module';
import { OrderListComponent } from './order-list/order-list.component';
import { OrderDetailsComponent } from './order-details/order-details.component';

@NgModule({
    declarations: [OrderListComponent, OrderDetailsComponent],
    imports: [CommonModule, OrderRoutingModule],
    providers: [],
})
export class OrderModule {}
