import { Injectable } from '@angular/core';
import { map, Observable, pluck, switchMap, tap } from 'rxjs';
import { ObservableStore } from '../core/observable-store';
import { ShoppingCart } from '../core/shopping-cart';
import { ItemModelAssembler } from './item-model.assembler';
import { ItemService } from './item.service';
import { ItemState } from './item.state';
import { ItemModel } from './models/item.model';

@Injectable()
export class ItemFacade {
    constructor(
        private service: ItemService,
        private assembler: ItemModelAssembler,
        private featureStore: ObservableStore<ItemState>,
        private cart: ShoppingCart
    ) {}

    getItem(sku: string): Observable<ItemModel> {
        return this.service.getItem(sku).pipe(
            map(x=>this.assembler.toItemModel(x))
        );
    }

    addItemToCart(sku: string, quantity: number): Promise<void> {
        this.cart.addItem(sku, quantity);
        return Promise.resolve();
    }
}
