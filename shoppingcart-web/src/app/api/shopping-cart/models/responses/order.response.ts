import { AddressResponse } from './address.response';
import { AuditableResponse } from '../../../auditable.response';
import { CustomerResponse } from './customer.response';
import { OrderItemResponse } from './order-item.response';

export interface OrderReponse extends AuditableResponse {
    orderResourceId: string;
    status: string;
    customer: CustomerResponse;
    address: AddressResponse;
    items: OrderItemResponse[];
}
