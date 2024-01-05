import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { AddressRequest } from '../api/shopping-cart/models/requests/address.request';
import { CustomerRequest } from '../api/shopping-cart/models/requests/customer.request';
import { OrderRequest } from '../api/shopping-cart/models/requests/order.request';
import { ShoppingCartClient } from '../api/shopping-cart/shopping-cart.client';
import { AddressInputModel } from '../cart/address-input.model';
import { CreateOrderModel } from '../cart/create-order.model';
import { CustomerInputModel } from '../cart/customer-input.model';
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

        // TODO
        model.customer = new CustomerInputModel();
        model.customer.firstName = "Jane";
        model.customer.lastName = "Doe";
        model.customer.emailAddress = "janedoe@test.com";
        model.customer.birthdate = "1975-1-1";

        model.address = new AddressInputModel();
        model.address.street = "Street";

        const request = assembleCreateOrderRequest(model);
        request.items = [{sku: "PAPPY-10", quantity: 1}]
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
            birthDate: "1975-01-01"
        } as CustomerRequest,
        address: {
            street: model.address!.street,
            city: model.address!.city,
            country: model.address?.country,
            zipCode: model.address?.zipCode,
        } as AddressRequest,
    } as OrderRequest;
};
