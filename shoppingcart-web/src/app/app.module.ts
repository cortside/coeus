import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppConfig } from 'src/environments/app-config';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthModule } from './auth/auth.module';
import { authenticationIntializerProvider } from './auth/authentication-initializer.provider';
import { AuthorizationRule, AUTHORIZATION_RULE } from './auth/authorization-rule';
import { USER_MANAGER_SETTINGS_TOKEN } from './auth/user-manager-settings.token';
import { CoreModule } from './core/core.module';
import { ItemModule } from './item/item.module';


export class TempRule implements AuthorizationRule {
    authorize(): void {
        console.log('i am temp rule');
    }
}


@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        HttpClientModule,
        CoreModule,

        // features
        AuthModule,
        ItemModule,

        // route
        AppRoutingModule,
    ],
    providers: [
        // auth
        { provide: USER_MANAGER_SETTINGS_TOKEN, useFactory: (config: AppConfig) => config.identity, deps: [AppConfig] },

        // app init
        authenticationIntializerProvider,

        //{provide: AUTHORIZATION_RULE, useClass: TempRule, multi: true}
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}

