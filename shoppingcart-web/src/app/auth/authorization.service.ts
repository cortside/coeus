import { inject, Inject, Injectable, Injector } from '@angular/core';
import { AuthorizationData } from './authorization-data';
import { AuthorizationRule, AUTHORIZATION_RULE, TempAuthorizationRule } from './authorization-rule';

@Injectable({
    providedIn: 'root',
})
export class AuthorizationService {
    private authorizations: Map<string, AuthorizationData>;
    constructor(private injector: Injector) {
        this.authorizations = new Map<string, AuthorizationData>();
    }

    register(serviceName: string, authorizationData: AuthorizationData) {
        this.authorizations.set(serviceName, authorizationData);
    }

    authorize() {
        console.log('running authorization rules');
        //this.authorizationRules.forEach(a=>a.authorize());'
        this.injector.get<TempAuthorizationRule>(TempAuthorizationRule).authorize();
    }
}
