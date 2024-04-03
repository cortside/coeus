import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { PagedModel } from '../common/paged.model';
import { ShoppingCart } from '../core/shopping-cart';
import { ItemModelAssembler } from './item-model.assembler';
import { ItemService } from './item.service';
import { ItemModel } from './models/item.model';

@Injectable()
export class ItemFacade {
    constructor(
        private service: ItemService,
        private assembler: ItemModelAssembler,
        //private featureStore: ObservableStore<ItemState>,
        private cart: ShoppingCart
    ) {}

    getItems(): Observable<PagedModel<ItemModel>> {
        return this.service.getItems().pipe(
            map(x=>this.assembler.toPagedItemModels(x))
        );
    }

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
