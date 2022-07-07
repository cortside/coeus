import { InjectionToken } from '@angular/core';
import { AuthorizationData } from './authorization-data';

export interface AuthorizationPolicy {
    authorize(policy: string, context: AuthorizationContext): boolean;
}

export const AUTHORIZATION_POLICY = new InjectionToken<AuthorizationPolicy>('AuthorizationRule');

export interface AuthorizationContext {
    authorizationData: AuthorizationData[];
}

// TODO: move
export class PermissionAuthorizationPolicy implements AuthorizationPolicy {
    authorize(policy: string, context: AuthorizationContext): boolean {
        const permission = context.authorizationData.find((d) => d.permissions.find((p) => p == policy));
        return permission != undefined;
    }
}
