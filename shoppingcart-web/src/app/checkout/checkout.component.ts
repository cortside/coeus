import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CartItemModel } from '../common/cart-item.model';
import { ShoppingCartService } from '../core/shopping-cart.service';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent {

  items$: Observable<CartItemModel[]>;
  constructor(private service: ShoppingCartService) {
    this.items$ = this.service.getCartItems();
  }

  checkout() {

  }
}
