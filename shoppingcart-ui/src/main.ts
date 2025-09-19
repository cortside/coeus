import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { loadConfigurations } from '@muziehdesign/angularcore';

loadConfigurations(['config.local.json']).then((config) => {
  bootstrapApplication(App, appConfig).catch((err) => console.error(err));
});
