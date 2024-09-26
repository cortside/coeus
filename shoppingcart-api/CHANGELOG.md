# Release 2024.09

## Library updates
* Cortside.Common -- 6.3
	* https://github.com/cortside/cortside.common/blob/develop/CHANGELOG.md
* Cortside.DomainEvent -- 6.3
	* https://github.com/cortside/cortside.domainevent/blob/develop/CHANGELOG.md
* Cortside.Health -- 6.1
	* https://github.com/cortside/cortside.health/blob/develop/CHANGELOG.md
* Cortside.MockServer -- 6.2
	* https://github.com/cortside/cortside.mockserver/blob/develop/CHANGELOG.md
* Cortside.RestApiClient -- 6.3
	* https://github.com/cortside/cortside.restapiclient/blob/develop/CHANGELOG.md
* Cortside.AspNetCore -- 6.3
	* https://github.com/cortside/cortside.aspnetcore/blob/develop/CHANGELOG.md

## Changes

* Updated to net8.0
* Updated all nuget packages
* Make use of AddScopedInterfacesBySuffix and AddSingletonClassesBySuffix
	* https://github.com/cortside/cortside.common/blob/release/6.3/CHANGELOG.md
*  Use of SearchBuilder classes now located in Cortside.AspNetCore
* Introduction of TestUtilities project with builder classes for dtos, models and entities
* Make use of WebApiFixture<T> for setting up integration tests
	* Allows for simpler test setup and for simpler configuration by only overriding what is needed
* Convert to using Asp.Versioning

## Migration notes

* Create migration for change to Outbox table from Cortside.DomainEvent.EntityFramework and for change to column name from CreateSubjectId to CreatedSubjectId from Cortside.AspNetCore.Audit
* Startup: IApiVersionDescriptionProvider is referenced from Asp.Versioning.ApiExplorer
* Controllers: ApiVersion and ApiVersionNeutral attributes needs to be referenced from Asp.Versioning
	* will need to remove nuget package reference to Microsoft.AspNetCore.Mvc.Versioning first
* ErrorsModel and ErrorModel should be referenced from Cortside.AspNetCore.Filters.Models
* Remove any references to
	* PolicyServer.Runtime.Client.AspNetCore
	* RestSharp
	* System.IdentityModel.*
	* IdentityServer4.*
