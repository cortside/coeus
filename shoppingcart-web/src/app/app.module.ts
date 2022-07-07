import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppConfig } from 'src/environments/app-config';

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

    ],
    bootstrap: [AppComponent],
})
export class AppModule {}

