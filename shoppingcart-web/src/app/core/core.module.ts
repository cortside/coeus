import { ErrorHandler, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationTokenInterceptor, AUTHORIZATION_POLICY, LOGGER, PermissionAuthorizationPolicy } from '@muziehdesign/core';
import { GlobalErrorHandler } from './global-error-handler';

@NgModule({
    declarations: [],
    imports: [CommonModule],
    providers: [
        { provide: LOGGER, useValue: window.console }, // TODO: use actual logger
        { provide: AUTHORIZATION_POLICY, useClass: PermissionAuthorizationPolicy, multi: true }, 
        { provide: ErrorHandler, useClass: GlobalErrorHandler },
        AuthenticationTokenInterceptor],
})
export class CoreModule {}
