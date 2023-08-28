import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationTokenInterceptor, AuthModule, AUTHORIZATION_POLICY, PermissionAuthorizationPolicy } from '@muziehdesign/auth';
import { AppConfig } from 'src/environments/app-config';

@NgModule({
    declarations: [],
    imports: [CommonModule, AuthModule],
    providers: [
        //authenticationIntializerProvider,
        {provide: AUTHORIZATION_POLICY, useClass: PermissionAuthorizationPolicy, multi: true},
        AuthenticationTokenInterceptor
    ],
})
export class CoreModule {}
