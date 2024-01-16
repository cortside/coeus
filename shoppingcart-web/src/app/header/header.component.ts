import { Component, OnInit } from '@angular/core';
import { AuthenticatedUser, AuthenticationService } from '@muziehdesign/core';
import { map, Observable } from 'rxjs';
import { ShoppingCartService } from '../core/shopping-cart.service';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
    user: AuthenticatedUser | undefined;
    itemCount$: Observable<number>;
    constructor(private auth: AuthenticationService, private cart: ShoppingCartService) {
      this.itemCount$ = this.cart.getCartItems().pipe(map((items)=>items.length));
    }

    async ngOnInit(): Promise<void> {
        this.user = await this.auth.getUser();
    }

    async signIn() {
      await this.auth.login();
      return false;
    }
}
