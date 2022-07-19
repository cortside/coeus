import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CartItemModel } from '../common/cart-item.model';
import { ShoppingCartService } from '../core/shopping-cart.service';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent implements OnInit {

  items$: Observable<CartItemModel[]>;
  constructor(private service: ShoppingCartService) { 
    this.items$ = this.service.getItems();
  }

  ngOnInit(): void { }
}
