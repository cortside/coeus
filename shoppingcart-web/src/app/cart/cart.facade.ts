import { Injectable } from "@angular/core";
import { firstValueFrom, Observable } from "rxjs";
import { CartItemModel } from "../common/cart-item.model";
import { OrderService } from "../core/order.service";
import { ShoppingCart } from "../core/shopping-cart";
import { CreateOrderModel } from "./create-order.model";

@Injectable()
export class CartFacade {
    constructor(private cart: ShoppingCart, private service: OrderService) { }

    getItems(): Observable<CartItemModel[]> {
        return this.cart.stateChanges();
    }

    async createOrder(model: CreateOrderModel): Promise<void> {
        const items = this.cart.getSnaptshot();
        const response = await firstValueFrom(this.service.createOrder(items, model));
        this.cart.clear();
    }

    async removeItem(sku: string): Promise<void> {
        this.cart.removeItem(sku);
        return Promise.resolve();
    }
}