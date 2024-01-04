import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { AddressRequest } from '../api/shopping-cart/models/requests/address.request';
import { CustomerRequest } from '../api/shopping-cart/models/requests/customer.request';
import { OrderRequest } from '../api/shopping-cart/models/requests/order.request';
import { ShoppingCartClient } from '../api/shopping-cart/shopping-cart.client';
import { CreateOrderModel } from '../cart/create-order.model';
import { OrderSummaryModel } from '../models/models';

@Injectable({
    providedIn: 'root',
})
export class OrderService {
    constructor(private client: ShoppingCartClient) {}

    debug(): void {
        console.log('debug');
    }

    createOrder(model: CreateOrderModel): Observable<OrderSummaryModel> {
        const request = assembleCreateOrderRequest(model);
        return this.client.createOrders(request).pipe(
            map((x) => {
                return {
                    orderResourceId: x.orderResourceId,
                    status: x.status,
                } as OrderSummaryModel;
            })
        );
    }
}

export const assembleCreateOrderRequest = (model: CreateOrderModel): OrderRequest => {
    return {
        customer: {
            firstName: model.customer!.firstName,
            lastName: model.customer!.lastName,
            email: model.customer?.emailAddress,
            birthDate: {
                year: 1975,
                month: 1,
                day: 1,
            },
        } as CustomerRequest,
        address: {
            street: model.address!.street,
            city: model.address!.city,
            country: model.address?.country,
            zipCode: model.address?.zipCode,
        } as AddressRequest,
    } as OrderRequest;
};
