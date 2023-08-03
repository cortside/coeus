import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '@muziehdesign/auth';
import { User } from 'oidc-client';
import { map, Observable } from 'rxjs';
import { ShoppingCartService } from '../core/shopping-cart.service';

@Component({
    selector: 'app-navigation',
    templateUrl: './navigation.component.html',
    styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent implements OnInit {
    quantity$: Observable<number>;
    user: User | undefined;
    constructor(private service: ShoppingCartService, private auth: AuthenticationService) {
        this.quantity$ = this.service.getCartItems().pipe(map((items) => items.map((i) => i.quantity).reduce((i, j) => i + j, 0)));
    }

    async ngOnInit() {
        this.user = await this.auth.getUser();
    }

    async signIn() {
        await this.auth.login();
    }
}
