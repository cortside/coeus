import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { Log, User, UserManager, UserManagerSettings } from 'oidc-client';
import { IdentityServerService } from '@muziehdesign/auth';
import { forkJoin, from, map, Observable, take } from 'rxjs';

import { AppModule } from './app/app.module';
import { AppConfig } from './environments/app-config';
import { environment } from './environments/environment';

const loadSettings = (urls: string[]): Observable<AppConfig> =>
    forkJoin(urls.map((url) => from(fetch(url).then((x) => x.json())) as Observable<AppConfig>)).pipe(
        map((configs: AppConfig[]) => Object.assign({}, ...configs) as AppConfig)
    );

loadSettings(environment.configurations)
    .pipe(take(1))
    .subscribe({
        next: async (appConfig: AppConfig) => {
            if (environment.production === true) {
                enableProdMode();
            }

           const ids = new IdentityServerService(appConfig.identity!);
           const user = await ids.bootstrap();

            // bootstrap
            const extraProviders = [
                { provide: IdentityServerService, useValue: ids },
                { provide: AppConfig, useValue: Object.freeze(appConfig) },
            ];

            platformBrowserDynamic(extraProviders)
                .bootstrapModule(AppModule)
                .catch((e: any) => {
                    document.body.innerHTML = e; // TODO
                });
        },
        error: (err) => {
            document.body.innerHTML = 'Failed to load application, please try again.';
            console.log(err);
        },
    });
