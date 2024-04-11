import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { CartItemModel } from '../models/cart-item.model';
import { ShoppingCart } from '../core/shopping-cart';

@Component({
    selector: 'app-checkout',
    templateUrl: './checkout.component.html',
    styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent {
    items$: Observable<CartItemModel[]>;
    constructor(private service: ShoppingCart) {
        this.items$ = this.service.stateChanges();
    }

    checkout(): void {
        console.log('test');
    }
}
