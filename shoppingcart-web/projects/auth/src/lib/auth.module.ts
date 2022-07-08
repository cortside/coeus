import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { EmptyComponent } from './empty/empty.component';
import { RouterModule } from '@angular/router';

@NgModule({
    declarations: [
       EmptyComponent
  ],
    imports: [CommonModule, RouterModule, AuthRoutingModule],
    providers: [],
})
export class AuthModule {}
