import { Injectable } from '@angular/core';
import { AuthenticationService } from '@muziehdesign/core';
import { map, Observable } from 'rxjs';
import { ShoppingCart } from '../core/shopping-cart';

@Injectable()
export class LayoutFacade {
    constructor(private auth: AuthenticationService, private cart: ShoppingCart) {}

    getUser() {
        return this.auth.getUser();
    }

    login() {
        return this.auth.login();
    }

    getCartCount(): Observable<number> {
        return this.cart.stateChanges().pipe(map((items) => items.length === 0 ? 0 : items.map((i) => i.quantity).reduce((a, b) => a + b)));
    }
}
