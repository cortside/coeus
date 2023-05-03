# IdentityServer implementation

## Setup
Run build.ps1 to create build.json

## Docker
docker run --restart=always -d `
	--name identityserver `
	-e SQL_SERVER=sqltestonlineapp.Acme.local `
	-e SQL_DATABASE=identityserver `
	-e SQL_USER=sa `
	-e SQL_PASSWORD=password1@ `
	-p 5000:5000 `
	acme/identityserver:1.0-develop

## client setup
There is a [template](docs/update-client-secret-template.sql) for how to update secrets for clients that are moving to production

## IdentityServer enum values
https://github.com/IdentityServer/IdentityServer4/blob/3ff3b46698f48f164ab1b54d124125d63439f9d0/src/Storage/src/Models/Enums.cs
