import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { catchError, map, Observable } from 'rxjs';
import { CatalogClient } from '../api/catalog/catalog.client';
import { ItemResponse } from '../api/catalog/models/responses/item.response';
import { PagedResponse } from '../api/paged.response';
import { ItemModel } from '../item/models/item.model';
import { PagedModel } from '../common/paged.model';
import { ShoppingCart } from '../core/shopping-cart';

@Injectable()
export class ItemService implements OnDestroy {

    constructor(
        private cart: ShoppingCart,
        private client: CatalogClient
    ) {

    }

    ngOnDestroy(): void {
        console.log('destroying item service');
    }

    getItem(sku: string): Observable<ItemModel> {
        return this.client.getItem(sku).pipe(
            catchError((error: HttpErrorResponse) => {
                console.log(error);
                throw new Error('not yet implemeted');
            }),
            map((x: ItemResponse) => assembleItemModel(x))
        );
    }

    getItems(): Observable<PagedModel<ItemModel>> {
        return this.client.getItems().pipe(
            map((x: PagedResponse<ItemResponse>) => {
                return {
                    totalItems: x.totalItems,
                    pageNumber: x.pageNumber,
                    pageSize: x.pageSize,
                    items: x.items.map((i) => assembleItemModel(i)),
                } as PagedModel<ItemModel>;
            })
        );
    }

    addToCart(sku: string, quantity: number): Promise<void> {
        this.cart.addItem(sku, quantity);
        return Promise.resolve();
    }
    
}

export const assembleItemModel = (response: ItemResponse): ItemModel => {
    return {
        itemId: response.itemId,
        name: response.name,
        sku: response.sku,
        unitPrice: response.unitPrice,
        imageUrl: response.imageUrl,
    } as ItemResponse;
};
