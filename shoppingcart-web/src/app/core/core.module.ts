import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationTokenInterceptor, AuthModule, AUTHORIZATION_POLICY, PermissionAuthorizationPolicy, USER_MANAGER_SETTINGS_TOKEN } from '@muziehdesign/auth';
import { AppConfig } from 'src/environments/app-config';

@NgModule({
    declarations: [],
    imports: [CommonModule, AuthModule],
    providers: [
        // auth
        { provide: USER_MANAGER_SETTINGS_TOKEN, useFactory: (config: AppConfig) => config.identity, deps: [AppConfig] },
        //authenticationIntializerProvider,
        {provide: AUTHORIZATION_POLICY, useClass: PermissionAuthorizationPolicy, multi: true},
        AuthenticationTokenInterceptor
    ],
})
export class CoreModule {}
