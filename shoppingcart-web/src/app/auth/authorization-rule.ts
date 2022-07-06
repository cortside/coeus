import { InjectionToken } from "@angular/core";

export interface AuthorizationRule {
    authorize():void;
}

export const AUTHORIZATION_RULE = new InjectionToken<AuthorizationRule>('AuthorizationRule');

export class TempAuthorizationRule implements AuthorizationRule {
    authorize(): void {
        console.log('temp');
    }
}
