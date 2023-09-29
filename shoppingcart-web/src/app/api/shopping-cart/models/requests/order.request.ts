import { AddressRequest } from './address.request';
import { CustomerRequest } from './customer.request';
import { ItemRequest } from './item.request';

export interface OrderRequest {
    customer: CustomerRequest;
    address: AddressRequest;
    items: ItemRequest[];
}
