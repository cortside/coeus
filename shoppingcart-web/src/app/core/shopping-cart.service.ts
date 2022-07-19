import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CartItemModel } from '../common/cart-item.model';
import { ItemService } from './item.service';

@Injectable({
    providedIn: 'root',
})
export class ShoppingCartService {
    private items = new BehaviorSubject<CartItemModel[]>([]);
    private items$ = this.items.asObservable();
    constructor() {}

    getItems(): Observable<CartItemModel[]> {
      return this.items$;
    }

    addItem(itemId: string, quantity: number) {
        this.items.next([...this.items.value, { itemId: itemId, quantity: quantity }]);
    }
}
