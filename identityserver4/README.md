# IdentityServer implementation Cortside

## Setup
Before running the Applicant Verification WebApi project, you will need to run the build.ps1 file. This will create a build.json in the src folder, which is required by the Applicant Verification WebApi at run time.


## Docker
docker run --restart=always -d `
	--name identityserver `
	-e SQL_SERVER=sqltestonlineapp.cortside.local `
	-e SQL_DATABASE=identityserver `
	-e SQL_USER=ids `
	-e SQL_PASSWORD=FQAS32gW `
	-p 5000:5000 `
	cortside.azurecr.io/identityserver:1.0-develop

## client setup
There is a [template](docs/update-client-secret-template.sql) for how to update secrets for clients that are moving to production

## IdentityServer enum values
https://github.com/IdentityServer/IdentityServer4/blob/3ff3b46698f48f164ab1b54d124125d63439f9d0/src/Storage/src/Models/Enums.cs
