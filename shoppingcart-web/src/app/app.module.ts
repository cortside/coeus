import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthenticationTokenInterceptor } from '@muziehdesign/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavigationComponent } from './navigation/navigation.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { HeaderComponent } from './header/header.component';
import { ProfileComponent } from './profile/profile.component';
import { FooterComponent } from './footer/footer.component';

@NgModule({
    declarations: [AppComponent, NavigationComponent, PageNotFoundComponent, HeaderComponent, ProfileComponent, FooterComponent],
    imports: [
        BrowserModule,
        HttpClientModule,

        // route
        AppRoutingModule,
    ],
    providers: [{ provide: HTTP_INTERCEPTORS, useClass: AuthenticationTokenInterceptor, multi: true }],
    bootstrap: [AppComponent],
})
export class AppModule {}
