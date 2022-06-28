import { Component } from '@angular/core';
import { AppConfig } from 'src/environments/app-config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'shoppingcart-web';

  constructor(private config: AppConfig) {
    console.log(config);
  }
}
