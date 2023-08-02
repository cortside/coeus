import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '@muziehdesign/auth';
import { User } from 'oidc-client';
import { map, Observable } from 'rxjs';
import { CartItemModel } from '../common/cart-item.model';
import { ShoppingCartService } from '../core/shopping-cart.service';

@Component({
    selector: 'app-navigation',
    templateUrl: './navigation.component.html',
    styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent {
    quantity$: Observable<number>;
    user: User| undefined;
    constructor(private service: ShoppingCartService, private auth: AuthenticationService) {
        this.quantity$ = this.service.getCartItems().pipe(map((items) => items.map((i) => i.quantity).reduce((i, j) => i + j, 0)));
        this.user = this.auth.user || undefined;
    }

    signIn() : void {
        this.auth.redirectToSignIn();
        this.user = this.auth.user || undefined;
    }
}
