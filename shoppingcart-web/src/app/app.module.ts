import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthenticationTokenInterceptor } from '@muziehdesign/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavigationComponent } from './navigation/navigation.component';

@NgModule({
    declarations: [AppComponent, NavigationComponent],
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
