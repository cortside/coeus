import { Component } from '@angular/core';
import { User } from 'oidc-client';
import { AppConfig } from 'src/environments/app-config';
import { AuthenticationService } from './auth/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'shoppingcart-web';
    user: User | undefined;
  constructor(private config: AppConfig, private authenticationService: AuthenticationService) {
    console.log(config);
    this.user = authenticationService.user;
  }
}
