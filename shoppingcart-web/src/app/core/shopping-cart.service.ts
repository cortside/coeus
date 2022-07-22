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

    getCartItems(): Observable<CartItemModel[]> {
      return this.items$;
    }

    addItem(itemSku: string, quantity: number) {
        const list = [...this.items.value];
        const item = list.find(i=>i.sku == itemSku);
        if(item) {
            item.quantity += quantity;
        } else {
            list.push({ sku: itemSku, quantity: quantity });
        }

        this.items.next(list);
    }
}
