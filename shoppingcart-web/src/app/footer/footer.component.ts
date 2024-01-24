import { Component } from '@angular/core';
import { AppConfig } from 'src/environments/app-config';
import { BuildInfo } from 'src/environments/build-info';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {
  
  buildInfo: BuildInfo;
  constructor(config: AppConfig) {
    this.buildInfo = config.build;
  }
}
