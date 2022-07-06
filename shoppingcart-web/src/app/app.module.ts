import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppConfig } from 'src/environments/app-config';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthModule } from './auth/auth.module';
import { authenticationIntializerProvider } from './auth/authentication-initializer.provider';
import { USER_MANAGER_SETTINGS_TOKEN } from './auth/user-manager-settings.token';
import { ItemModule } from './item/item.module';
import { OrderModule } from './order/order.module';

@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        HttpClientModule,

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
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
