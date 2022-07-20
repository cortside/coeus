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

    addItem(itemSku: string, quantity: number) {
        const item = this.items.value.find(i=>i.sku == itemSku);
        if(item) {
            item.quantity += quantity;
            return;
        }

        this.items.next([...this.items.value, { sku: itemSku, quantity: quantity }]);
    }
}
