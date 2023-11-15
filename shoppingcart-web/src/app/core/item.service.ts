import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { CatalogClient } from '../api/catalog/catalog.client';
import { ItemResponse } from '../api/catalog/models/responses/item.response';
import { PagedResponse } from '../api/paged.response';
import { ItemModel } from '../catalog/models/item.model';
import { PagedModel } from '../common/paged.model';

@Injectable({
    providedIn: 'root',
})
export class ItemService {
    constructor(private client: CatalogClient) {}

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
