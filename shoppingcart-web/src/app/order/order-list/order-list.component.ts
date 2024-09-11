import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderService } from 'src/app/core/order.service';
import { OrderSummaryModel } from 'src/app/models/models';
import { PagedModel } from 'src/app/models/paged.model';

@Component({
    selector: 'app-order-list',
    templateUrl: './order-list.component.html',
    styleUrls: ['./order-list.component.scss'],
})
export class OrderListComponent {
    orders$: Observable<PagedModel<OrderSummaryModel>>;
    constructor(private service: OrderService) {
        this.orders$ = service.getOrders();
    }
}
