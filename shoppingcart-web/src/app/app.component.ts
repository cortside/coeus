import { Component } from '@angular/core';
import { User } from 'oidc-client';
import { AppConfig } from 'src/environments/app-config';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent {
    title = 'shoppingcart-web';
    user: User | null = null;
    constructor(private config: AppConfig) {

    }
}
