import { ItemResponse } from "../api/catalog/models/responses/item.response";

export interface ItemState {
    item: ItemResponse | undefined;
    // TODO: pagination parameters
}