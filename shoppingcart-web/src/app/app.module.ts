import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthenticationTokenInterceptor, LOGGER } from '@muziehdesign/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { HeaderComponent } from './header/header.component';
import { ProfileComponent } from './profile/profile.component';
import { FooterComponent } from './footer/footer.component';
import { CoreModule } from './core/core.module';
import { APP_INITIALIZER } from '@angular/core';
import { initializeApplication } from './app-initializer';

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
        { provide: APP_INITIALIZER, useFactory: initializeApplication, multi: true, deps: [LOGGER]},
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
