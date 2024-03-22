import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationTokenInterceptor, AUTHORIZATION_POLICY, LOGGER, PermissionAuthorizationPolicy } from '@muziehdesign/core';

@NgModule({
    declarations: [],
    imports: [CommonModule],
    providers: [
        { provide: LOGGER, useValue: window.console }, // TODO: use actual logger
        { provide: AUTHORIZATION_POLICY, useClass: PermissionAuthorizationPolicy, multi: true }, 
        AuthenticationTokenInterceptor],
})
export class CoreModule {}
