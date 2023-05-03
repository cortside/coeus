# setup okta for integration with identityserver

https://deblokt.com/2019/09/30/06-identityserver4-external-providers/

* https://dev-1303959-admin.okta.com/admin/apps/active
* create app integration
    * oidc
    * spa
    * name (i.e. identityserver)
    * grant types -- only implicit (hybrid)
    * don't require consent
    * sign-in redirect url
        * http://localhost:5004/signin-oidc-okta
    * sigh-out redirect url
        * http://localhost:5004/signout-oidc
    * login initiated by app only
    * initiate login url -- left blank
    * access with federation -- enabled
* you will need the clientId from the new application for configuration in ids
    * in appsettings.json/config.json
    ```json
    "Okta": {
        "Authority": "https://dev-1303959-admin.okta.com",
        "ClientId": "0oa5kzepxsyyNljDy5d7",
        "CallbackPath": "/signin-oidc-okta"
    },
    ```

# identity server

* startup checks for configuration values to know which external providers to allow:
    ```json
        "azureActiveDirectory": {
            "authority": "https://login.microsoftonline.com/common",
            "clientId": "d0abb606-ebd5-4939-903f-f2c702ff40b0"
        },
        "Okta": {
            "Authority": "https://dev-1303959-admin.okta.com",
            "ClientId": "0oa5kzepxsyyNljDy5d7",
            "CallbackPath": "/signin-oidc-okta"
        },
    ```
* both may be enabled at the same time
* there will be an "employee" login for each external provider configured
