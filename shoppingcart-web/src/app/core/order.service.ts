import { Injectable } from '@angular/core';
import { map, tap } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { AddressRequest } from '../api/shopping-cart/models/requests/address.request';
import { CustomerRequest } from '../api/shopping-cart/models/requests/customer.request';
import { OrderRequest } from '../api/shopping-cart/models/requests/order.request';
import { OrderReponse } from '../api/shopping-cart/models/responses/order.response';
import { ShoppingCartClient } from '../api/shopping-cart/shopping-cart.client';
import { AddressInputModel } from '../cart/address-input.model';
import { CreateOrderModel } from '../cart/create-order.model';
import { CustomerInputModel } from '../cart/customer-input.model';
import { CartItemModel } from '../models/cart-item.model';
import { CustomerModel, OrderSummaryModel } from '../models/models';
import { PagedModel } from '../models/paged.model';

@Injectable({
    providedIn: 'root',
})
export class OrderService {
    constructor(private client: ShoppingCartClient) {}

    createOrder(items: CartItemModel[], model: CreateOrderModel): Observable<OrderSummaryModel> {
        // TODO
        model.customer = new CustomerInputModel();
        model.customer.firstName = "Jane";
        model.customer.lastName = "Doe";
        model.customer.emailAddress = "janedoe@test.com";
        model.customer.birthdate = "1975-1-1";

        model.address = new AddressInputModel();
        model.address.street = "Street";

        const request = assembleCreateOrderRequest(model);
        request.items = items;
        return this.client.createOrders(request).pipe(
            map((x) => {
                return {
                    orderResourceId: x.orderResourceId,
                    status: x.status,
                } as OrderSummaryModel;
            })
        );
    }

    getOrders(): Observable<PagedModel<OrderSummaryModel>> {
        return this.client.getOrders().pipe(
            map((x)=> {
                const items = x.items.map(y=>assembleOrderSummaryModel(y));
                return {
                    totalItems: x.totalItems,
                    pageNumber: x.pageNumber,
                    pageSize: x.pageSize,
                    items: items
                } as PagedModel<OrderSummaryModel>;
            })
        );
    }
}

export const assembleOrderSummaryModel = (response: OrderReponse): OrderSummaryModel => {
    return {
        orderResourceId: response.orderResourceId,
        status: response.status,
        customer: {
            customerResourceId: response.customer.customerResourceId,
            firstName: response.customer.firstName,
            lastName: response.customer.lastName,
            email: response.customer.email
        } as CustomerModel
    } as OrderSummaryModel;
};

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
