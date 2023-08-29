import { inject, Inject, Injectable, Injector } from '@angular/core';
import { AuthorizationData } from './authorization-data';
import { AuthorizationPolicy, AUTHORIZATION_POLICY } from './authorization-policy';

@Injectable({
    providedIn: 'root',
})
export class AuthorizationService {
    private authorizations: Map<string, AuthorizationData>;
    constructor(@Inject(AUTHORIZATION_POLICY) private policies: AuthorizationPolicy[]) {
        this.authorizations = new Map<string, AuthorizationData>();
    }

    set(data: Map<string, AuthorizationData>){
        this.authorizations = data;
    }

    /**
     * Resets all authorization data stored in the service.
     */
    reset() {
        this.authorizations = new Map<string, AuthorizationData>();
    }

    authorize(policyName: string): boolean {
        const data = [...this.authorizations.values()];
        const policy = this.policies.find((a) => a.authorize(policyName, { authorizationData: data }) == false);
        return policy == undefined;
    }
}
