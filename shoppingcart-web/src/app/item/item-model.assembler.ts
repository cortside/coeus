import { ItemResponse } from "../api/catalog/models/responses/item.response";
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
}