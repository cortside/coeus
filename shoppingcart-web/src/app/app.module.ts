import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthenticationService, AuthenticationTokenInterceptor, AuthorizationService, LOGGER } from '@muziehdesign/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { HeaderComponent } from './header/header.component';
import { ProfileComponent } from './profile/profile.component';
import { FooterComponent } from './footer/footer.component';
import { CoreModule } from './core/core.module';
import { APP_INITIALIZER } from '@angular/core';
import { initializeApplication, initializeAuthorization } from './app-initializer';
import { ShoppingCartClient } from './api/shopping-cart/shopping-cart.client';

@NgModule({
    declarations: [AppComponent, PageNotFoundComponent, HeaderComponent, ProfileComponent, FooterComponent],
    imports: [
        BrowserModule,
        HttpClientModule,
        CoreModule,

        // route
        AppRoutingModule,
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthenticationTokenInterceptor, multi: true },
        { provide: APP_INITIALIZER, useFactory: initializeApplication, multi: true, deps: [LOGGER] },
        { provide: APP_INITIALIZER, useFactory: initializeAuthorization, multi: true, deps: [AuthenticationService, AuthorizationService, ShoppingCartClient] },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
