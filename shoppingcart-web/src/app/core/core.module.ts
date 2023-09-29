import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationTokenInterceptor, AUTHORIZATION_POLICY, PermissionAuthorizationPolicy } from '@muziehdesign/core';

@NgModule({
    declarations: [],
    imports: [CommonModule],
    providers: [{ provide: AUTHORIZATION_POLICY, useClass: PermissionAuthorizationPolicy, multi: true }, AuthenticationTokenInterceptor],
})
export class CoreModule {}
