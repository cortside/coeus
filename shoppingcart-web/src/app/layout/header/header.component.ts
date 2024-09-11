import { Component, OnInit } from '@angular/core';
import { AuthenticatedUser } from '@muziehdesign/core';
import { Observable } from 'rxjs';
import { LayoutFacade } from '../layout.facade';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
    user: AuthenticatedUser | undefined;
    itemCount$: Observable<number>;
    constructor(
        private facade: LayoutFacade
    ) {
        this.itemCount$ = this.facade.getCartCount();
    }

    async ngOnInit(): Promise<void> {
        this.user = await this.facade.getUser();
    }

    async signIn() {
        await this.facade.login();
        return false;
    }
}
