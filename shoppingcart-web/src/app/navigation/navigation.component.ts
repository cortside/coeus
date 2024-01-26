import { Component, OnInit } from '@angular/core';
import { AuthenticatedUser, AuthenticationService } from '@muziehdesign/core';
import { map, Observable } from 'rxjs';
import { ShoppingCart } from '../core/shopping-cart';

@Component({
    selector: 'app-navigation',
    templateUrl: './navigation.component.html',
    styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent implements OnInit {
    quantity$: Observable<number>;
    user: AuthenticatedUser | undefined;
    constructor(
        private service: ShoppingCart,
        private auth: AuthenticationService
    ) {
        this.quantity$ = this.service.getCartItems().pipe(map((items) => items.map((i) => i.quantity).reduce((i, j) => i + j, 0)));
    }

    async ngOnInit(): Promise<void> {
        this.user = await this.auth.getUser();
    }

    async signIn(): Promise<void> {
        await this.auth.login();
    }
}
