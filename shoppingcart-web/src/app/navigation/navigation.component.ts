import { Component, OnInit } from '@angular/core';
import { map, Observable } from 'rxjs';
import { CartItemModel } from '../common/cart-item.model';
import { ShoppingCartService } from '../core/shopping-cart.service';

@Component({
    selector: 'app-navigation',
    templateUrl: './navigation.component.html',
    styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent implements OnInit {
    quantity$: Observable<number>;
    constructor(private service: ShoppingCartService) {
        this.quantity$ = this.service.getCartItems().pipe(map((items) => items.map((i) => i.quantity).reduce((i, j) => i + j, 0)));
    }

    ngOnInit(): void {}
}
