import { Component, OnInit } from '@angular/core';
import { AuthenticatedUser, AuthenticationService } from '@muziehdesign/core';
import { map, Observable } from 'rxjs';
import { ShoppingCartService } from '../core/shopping-cart.service';

@Component({
    selector: 'app-navigation',
    templateUrl: './navigation.component.html',
    styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent implements OnInit {
    quantity$: Observable<number>;
    user: AuthenticatedUser | undefined;
    constructor(
        private service: ShoppingCartService,
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
