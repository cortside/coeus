import { AuthorizationService } from '@muziehdesign/core';
import { EMPTY, firstValueFrom, map, Observable, of } from 'rxjs';
import { ItemResponse } from '../api/catalog/models/responses/item.response';
import { ItemService } from '../core/item.service';
import { ObservableStore } from '../core/observable-store';
import { SHOPPING_CART_PERMISSIONS } from '../core/permissions';

export interface ItemDetailState {
    item: ItemResponse;
    authorizations: string[];
}

export class ItemDetailStore extends ObservableStore<ItemDetailState> {
    private item$: Observable<ItemResponse | undefined> = of(undefined);

    constructor(
        private service: ItemService,
        private authorization: AuthorizationService
    ) {
        super({} as ItemDetailState);
    }


    // queries
    getAddItemToCartModel(): Observable<AddItemToCartModel> {
        return this.stateChanges().pipe(
            map((x) => {
                return {
                    quantity: 1,
                    display: x.authorizations.includes(SHOPPING_CART_PERMISSIONS.createOrder),
                } satisfies AddItemToCartModel;
            })
        );
    }

    // actions
    async loadItem(sku: string): Promise<void> {
        this.item$ = this.service.getItem(sku);
        const item = await firstValueFrom(this.service.getItem(sku));
        this.patch({ item: item });
    }

    async initialize(item: ItemResponse): Promise<void> {
        this.set({item, authorizations: []})
    }
}

export interface AddItemToCartModel {
    quantity: number;
    display: boolean;
}
