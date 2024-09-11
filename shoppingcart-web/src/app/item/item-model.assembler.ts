import { ItemResponse } from "../api/catalog/models/responses/item.response";
import { PagedResponse } from "../api/paged.response";
import { PagedModel } from "../models/paged.model";
import { ItemModel } from "./models/item.model";

export class ItemModelAssembler {
    toItemModel(item: ItemResponse) : ItemModel {
        return {
            itemId: item.itemId,
            name: item.name,
            sku: item.sku,
            unitPrice: item.unitPrice,
            imageUrl: item.imageUrl
        };
    }

    toPagedItemModels(response: PagedResponse<ItemResponse>): PagedModel<ItemModel> {
        return {
            totalItems: response.totalItems,
            pageNumber: response.pageNumber,
            pageSize: response.pageSize,
            items: response.items.map((i) => this.toItemModel(i)),
        } satisfies PagedModel<ItemModel>;
    }
}