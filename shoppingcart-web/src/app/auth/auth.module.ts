import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { EmptyComponent } from './empty/empty.component';

@NgModule({
    declarations: [
       EmptyComponent
  ],
    imports: [CommonModule, AuthRoutingModule],
    providers: [],
})
export class AuthModule {}
