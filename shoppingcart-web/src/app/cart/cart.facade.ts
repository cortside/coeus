import { Injectable } from "@angular/core";
import { AuthenticationService } from "@muziehdesign/core";
import { firstValueFrom, Observable } from "rxjs";
import { CartItemModel } from "../models/cart-item.model";
import { OrderService } from "../core/order.service";
import { ShoppingCart } from "../core/shopping-cart";
import { CreateOrderModel } from "./create-order.model";

@Injectable()
export class CartFacade {
    constructor(private cart: ShoppingCart, private service: OrderService, private auth: AuthenticationService) { }

    getItems(): Observable<CartItemModel[]> {
        return this.cart.stateChanges();
    }

    async loginIfNeeded(): Promise<boolean> {
        const authenticated = await this.auth.isAuthenticated();
        if(!authenticated) {
            await this.auth.login(); // TODO: redirect to state is not working
            return true;
        }

        return false;
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