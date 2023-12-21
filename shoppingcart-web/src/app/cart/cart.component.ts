import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { CartItemModel } from '../common/cart-item.model';
import { ShoppingCartService } from '../core/shopping-cart.service';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-cart',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './cart.component.html',
    styleUrls: ['./cart.component.scss'],
})
export class CartComponent {
    items$: Observable<CartItemModel[]>;
    constructor(private service: ShoppingCartService) {
        this.items$ = this.service.getCartItems();
    }

    checkout(): void {
        console.log('test');
    }
}
