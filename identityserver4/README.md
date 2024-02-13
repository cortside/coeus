# IdentityServer 

## Setup
Before running the project, you will need to run the build.ps1 file. This will create a build.json in the src folder, which is required by the api at run time.


## Docker
docker run --restart=always -d `
	--name identityserver `
	-e SQL_SERVER=localhost `
	-e SQL_DATABASE=identityserver `
	-e SQL_USER=sa `
	-e SQL_PASSWORD=abc123 `
	-p 5000:5000 `
	identityserver:1.0-develop

## client setup
There is a [template](docs/update-client-secret-template.sql) for how to update secrets for clients that are moving to production

## IdentityServer enum values
https://github.com/IdentityServer/IdentityServer4/blob/3ff3b46698f48f164ab1b54d124125d63439f9d0/src/Storage/src/Models/Enums.cs

## Generating new token signing certificate

NOTE: certificates should not be committed as it can be possible exposure to token forgeries.

```powershell
$Certificate=New-SelfsignedCertificate `
	-KeyExportPolicy Exportable `
	-Subject "CN=IdentityServerSignature" `
	-KeySpec Signature `
	-KeyAlgorithm RSA `
	-KeyLength 2048 `
	-HashAlgorithm SHA256 `
	-CertStoreLocation "cert:\LocalMachine\My" `
	-NotAfter (Get-Date).AddYears(10)

$Pwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText 
Export-PfxCertificate -Cert $Certificate -FilePath "IdentityServer.pfx" -Password $Pwd
```

## Generating hashed client secret values (bash)
echo -n 'plaintextpassword' | openssl dgst -binary -sha256 | openssl base64

Example of setting up multiple external providers:

```json
    "externalProviders": [
        {
            "scheme": "AAD",
            "displayName": "Azure Active Directory",
            "authority": "https://login.microsoftonline.com/common",
            "clientId": "clientId",
            "callbackPath": "/signin-oidc",
            "scopes": [ "openid", "profile" ],
            "providerIdClaim": "nameid",
            "usernameclaim": "upn",
            "claims": [ "email", "family_name", "given_name", "groups", "name" ]
        },
        {
            "scheme": "Okta",
            "displayName": "Okta",
            "authority": "https://oktaauthority",
            "clientId": "clientId",		
            "callbackPath": "/signin-oidc-okta",
            "scopes": [ "openid", "profile", "groups", "email", "phone" ],
            "providerIdClaim": "nameid",
            "usernameClaim": "upn",
            "claims": [ "email", "family_name", "given_name", "groups", "name", "locale", "zoneinfo" ],
            "AddIdTokenHint": true
        }
    ],
```