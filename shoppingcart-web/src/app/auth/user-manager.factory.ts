import { FactoryProvider } from "@angular/core";
import { UserManager, UserManagerSettings } from "oidc-client";

// helpful documentations on oidc-client-js
// https://github.com/IdentityModel/oidc-client-js/wiki/Home/2fc5816d3ac19b392a07b136f0179c1be254d131
// https://gist.github.com/davidamidon/24c8a6980e116e62f781be4d6239d10d
// https://github.com/IdentityModel/oidc-client-js/issues/339
export const userManagerFactory = (settings: UserManagerSettings) : UserManager => {
    const defaults = <UserManagerSettings> {
        checkSessionInterval: 10000,
        monitorSession: true,
        automaticSilentRenew: true,
        response_type: 'id_token token'
    };
    return new UserManager(Object.assign({}, defaults, settings));
};
