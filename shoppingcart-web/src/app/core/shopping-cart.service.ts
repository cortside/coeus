import { DOCUMENT } from '@angular/common';
import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CartItemModel } from '../common/cart-item.model';

@Injectable({
    providedIn: 'root',
})
export class ShoppingCartService {
    private items:BehaviorSubject<CartItemModel[]>;
    private items$:Observable<CartItemModel[]>;
    readonly ITEMS_KEY = 'ShoppingCartService.items';

    constructor(@Inject(DOCUMENT) private document: Document) {
        const existing = JSON.parse(this.document.defaultView?.localStorage.getItem(this.ITEMS_KEY) || "[]");
        this.items = new BehaviorSubject<CartItemModel[]>(existing);
        this.items$ = this.items.asObservable();
    }

    getCartItems(): Observable<CartItemModel[]> {
        return this.items$;
    }

    getCurrentCartItems(): CartItemModel[] {
        return [...this.items.value];
    }

    addItem(itemSku: string, quantity: number): void {
        const list = [...this.items.value];
        const item = list.find((i) => i.sku == itemSku);
        if (item) {
            item.quantity += quantity;
        } else {
            list.push({ sku: itemSku, quantity: quantity });
        }

        this.document.defaultView?.localStorage.setItem(this.ITEMS_KEY, JSON.stringify(list));
        this.items.next(list);
    }

    clear(): void {
        this.document.defaultView?.localStorage.removeItem(this.ITEMS_KEY);
        this.items.next([]);
    }
}
