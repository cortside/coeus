import { AddressModel } from './create-address.model';

export class CustomerModel {
    firstName?: string;
    lastName?: string;
    email?: string;
    birthDate?: Date;
}

export class OrderModel {
    customer?: CustomerModel;
    address?: AddressModel;
    items?: CreateOrderItemModel[];
}

export class CreateOrderItemModel {
    sku?: string;
    quantity?: number;
}
