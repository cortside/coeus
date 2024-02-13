import { AuditableResponse } from '../../../auditable.response';

export interface OrderItemResponse extends AuditableResponse {
    orderItemId: number;
    itemId: string;
    sku: string;
    quantity: number;
    unitPrice: number;
}
