import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthenticationService, AuthenticationTokenInterceptor, AuthorizationData, AuthorizationService } from '@muziehdesign/auth';
import { firstValueFrom } from 'rxjs';
import { AppConfig } from 'src/environments/app-config';
import { ShoppingCartClient } from './api/shopping-cart/shopping-cart.client';
import { initializeApplication } from './app-initializer';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { ItemModule } from './item/item.module';


@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        HttpClientModule,
        CoreModule,

        ItemModule,

        // route
        AppRoutingModule,
    ],
    providers: [
        { provide: APP_INITIALIZER, useFactory: initializeApplication, deps: [AuthenticationService, ShoppingCartClient, AuthorizationService], multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: AuthenticationTokenInterceptor, multi: true}
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
