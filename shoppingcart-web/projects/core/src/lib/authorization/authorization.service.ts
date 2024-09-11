import { inject, Inject, Injectable, Injector } from '@angular/core';
import { AuthorizationData } from './authorization-data';
import { AuthorizationPolicy, AUTHORIZATION_POLICY } from './authorization-policy';

@Injectable({
    providedIn: 'root',
})
export class AuthorizationService {
    private authorizations: Map<string, AuthorizationData>;
    constructor(@Inject(AUTHORIZATION_POLICY) private authorizationPolicies: AuthorizationPolicy[]) {
        this.authorizations = new Map<string, AuthorizationData>();
    }

    set(data: Map<string, AuthorizationData>) {
        this.authorizations = data;
    }

    register(key: string, data: AuthorizationData) {
        this.authorizations.set(key, data);
    }

    /**
     * Resets all authorization data stored in the service.
     */
    reset() {
        this.authorizations = new Map<string, AuthorizationData>();
    }

    // authorizes based on snapshot
    authorize(policy: string): boolean {
        const result = this.authorizePolicies([policy]);
        return result.length > 0;
    }

    authorizePolicies(policies: string[]): string[] {
        const data = [...this.authorizations.values()];
        const results: string[] = [];
        for (const policy of policies) {
            const result = data.find((d) => d.permissions.includes(policy));
            if (result) {
                results.push(policy);
            }
        }
        return results;
    }
}
