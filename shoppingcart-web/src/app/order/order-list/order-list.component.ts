import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedModel } from 'src/app/common/paged.model';
import { OrderService } from 'src/app/core/order.service';
import { OrderSummaryModel } from 'src/app/models/models';

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
