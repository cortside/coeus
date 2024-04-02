import { Component, OnInit } from '@angular/core';
import { AuthenticatedUser, AuthenticationService } from '@muziehdesign/core';
import { map, Observable } from 'rxjs';
import { ShoppingCart } from '../core/shopping-cart';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
    user: AuthenticatedUser | undefined;
    itemCount$: Observable<number>;
    constructor(private auth: AuthenticationService, private cart: ShoppingCart) {
      this.itemCount$ = this.cart.stateChanges().pipe(map((items)=>items.length));
    }

    async ngOnInit(): Promise<void> {
        this.user = await this.auth.getUser();
    }

    async signIn() {
      await this.auth.login();
      return false;
    }
}
