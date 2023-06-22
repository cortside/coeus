import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { User, UserManager, UserManagerSettings } from 'oidc-client';
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

            // authentication
            let currentUser: User | null = null;
            const identityDefaults = <UserManagerSettings>{
                checkSessionInterval: 10000,
                monitorSession: true,
                automaticSilentRenew: true,
                response_type: 'id_token token',
            };
            const userManager = new UserManager(Object.assign({}, identityDefaults, appConfig.identity));
            // intercept silent redirect and halt actual bootstrap
            if (userManager.settings.silent_redirect_uri && window.location.href.indexOf(userManager.settings.silent_redirect_uri) > -1) {
                return userManager.signinSilentCallback().then(() => {});
            }
            //handle signin callback
            if (userManager.settings.redirect_uri && window.location.href.indexOf(userManager.settings.redirect_uri) > -1) {
                currentUser = await userManager.signinRedirectCallback();
                window.history.replaceState({}, window.document.title, currentUser.state);
            } /*else {
                // validate user existence
                const currentUser = await userManager.signinSilent().catch(() => null);
                if (currentUser == null) {
                    // TODO: move this logic, user can be authenticated
                    return userManager.signinRedirect({ state: window.location.href });
                }
            } */

            // bootstrap
            const extraProviders = [
                { provide: UserManager, useValue: userManager },
                { provide: AppConfig, useValue: Object.freeze(appConfig) },
            ];

            platformBrowserDynamic(extraProviders)
                .bootstrapModule(AppModule)
                .catch((e) => {
                    document.body.innerHTML = e; // TODO
                });
        },
        error: (err) => {
            document.body.innerHTML = 'Failed to load application, please try again.';
            console.log(err);
        },
    });
