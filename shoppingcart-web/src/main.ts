import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { Log, User, UserManager, UserManagerSettings } from 'oidc-client';
import { forkJoin, from, map, Observable, take } from 'rxjs';

import { AppModule } from './app/app.module';
import { AppConfig } from './environments/app-config';
import { environment } from './environments/environment';

const loadSettings = (urls: string[]): Observable<AppConfig> =>
    forkJoin(urls.map((url) => from(fetch(url).then((x) => x.json())) as Observable<AppConfig>)).pipe(
        map((configs: AppConfig[]) => Object.assign({}, ...configs) as AppConfig)
    );

const userManagerFactory = (config: AppConfig): UserManager => {
    const c = config.identity!;
    Log.level = config.identity!.logLevel;

    return new UserManager({
        authority: c.authority,
        client_id: c.client_id,
        redirect_uri: c.redirect_uri,
        silent_redirect_uri: c.silent_redirect_uri,
        post_logout_redirect_uri: '/', // TODO
        response_type: 'id_token token',
        scope: config.identity!.scope,
        //automaticSilentRenew: config.identity!.automaticSilentRenew,
        //filterProtocolClaims: config.identity!.filterProtocolClaims,
        loadUserInfo: true,
        monitorSession: true,
    });
};

loadSettings(environment.configurations)
    .pipe(take(1))
    .subscribe({
        next: async (appConfig: AppConfig) => {
            if (environment.production === true) {
                enableProdMode();
            }

            const userManager = userManagerFactory(appConfig);

            // intercept silent redirect and halt actual bootstrap
            if (userManager.settings.silent_redirect_uri != null && window.location.href.indexOf(userManager.settings.silent_redirect_uri) > -1) {
                return userManager.signinSilentCallback();
            }

            // handle signin callback
            if (userManager.settings.redirect_uri != null && window.location.href.indexOf(userManager.settings.redirect_uri) > -1) {
                const redirectedUser = await userManager.signinRedirectCallback();
                window.history.replaceState({}, window.document.title, redirectedUser.state);
            } else {
                // validate user existence
                const user: User | null = await userManager.signinSilent().catch(() => null);

                if (user == null) {
                    return userManager.signinRedirect({ state: window.location.href });
                }
            }

            // bootstrap
            const extraProviders = [
                { provide: UserManager, useValue: userManager },
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
