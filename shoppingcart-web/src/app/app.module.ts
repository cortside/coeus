import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthenticationService, AuthenticationTokenInterceptor, AuthorizationService } from '@muziehdesign/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { NavigationComponent } from './navigation/navigation.component';

@NgModule({
    declarations: [AppComponent, NavigationComponent],
    imports: [
        BrowserModule,
        HttpClientModule,

        // route
        AppRoutingModule,
    ],
    providers: [
        //{ provide: APP_INITIALIZER, useFactory: initializeApplication, deps: [AuthenticationService, ShoppingCartClient, AuthorizationService], multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: AuthenticationTokenInterceptor, multi: true },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
