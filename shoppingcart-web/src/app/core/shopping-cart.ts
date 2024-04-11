import { DOCUMENT } from '@angular/common';
import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CartItemModel } from '../models/cart-item.model';

@Injectable({
    providedIn: 'root',
})
export class ShoppingCart {
    private items: BehaviorSubject<CartItemModel[]>;
    readonly ITEMS_KEY = 'ShoppingCart.items';

    constructor(@Inject(DOCUMENT) private document: Document) {
        const existing = JSON.parse(this.document.defaultView?.localStorage.getItem(this.ITEMS_KEY) || '[]');
        this.items = new BehaviorSubject<CartItemModel[]>(existing);
    }

    stateChanges(): Observable<CartItemModel[]> {
        return this.items.asObservable();
    }

    getSnaptshot(): CartItemModel[] {
        return [...this.items.value];
    }

    removeItem(sku: string): void {
        const list = this.getSnaptshot();
        const i = list.findIndex((i) => i.sku === sku);
        if (i > -1) {
            list.splice(i, 1)
            this.document.defaultView?.localStorage.setItem(this.ITEMS_KEY, JSON.stringify(list));
            this.items.next(list);
        }
    }

    addItem(itemSku: string, quantity: number): void {
        const list = [...this.items.value];
        const item = list.find((i) => i.sku == itemSku);
        if (item) {
            item.quantity += quantity;
        } else {
            list.push({ sku: itemSku, quantity: quantity, unitPrice: 0 });
        }

        this.document.defaultView?.localStorage.setItem(this.ITEMS_KEY, JSON.stringify(list));
        this.items.next(list);
    }

    clear(): void {
        this.document.defaultView?.localStorage.removeItem(this.ITEMS_KEY);
        this.items.next([]);
    }
}
