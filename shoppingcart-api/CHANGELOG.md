# Release 2025.03

## Library updates

* amqptools


* Cortside.Bowdlerizer

    * Changes
        * Updated powershell scripts to latest versions from coeus/shoppingcart-api
        * Standardized library build files and resolved code coverage issues
        * Update target framework to net8.0
        * Update all dependency nuget packages
        * Add support for appsettings configured mask strategy ()
        * Add additional tests to raise code coverage
        * Add/Fix build badges
        * Transition to use Shouldly instead of FluentAssertions


* serilog.bowdlerizer

    * Changes
        * Updated powershell scripts to latest versions from coeus/shoppingcart-api
        * Standardized library build files and resolved code coverage issues
        * Update target framework to net8.0
        * Update all dependency nuget packages
        * Add/Fix build badges
        * Transition to use Shouldly instead of FluentAssertions


* Cortside.Common

    * Changes
        * Updated powershell scripts to latest versions from coeus/shoppingcart-api
        * Standardized library build files and resolved code coverage issues
        * Update target framework to net8.0
        * Update all dependency nuget packages
        * Add/Fix build badges
        * Transition to use Shouldly instead of FluentAssertions
        * Fixed annotation in Cortside.Common.Testing.EntityFramework to publish nuget package despite Test in the name
        * Add PreconditionFailedResponseException to be used by Cortside.AspNetCore MessageExceptionResponseFilter
        * Add Microsoft.Extensions.Logging ILogger extension methods to make adding properties to logging context (similar style to how it's done in Serilog, without having to rely on serilog deep into a solution)

* Cortside.Health

    * Changes
        * Updated powershell scripts to latest versions from coeus/shoppingcart-api
        * Standardized library build files and resolved code coverage issues
        * Update target framework to net8.0
        * Update all dependency nuget packages
        * Add/Fix build badges
        * Transition to use Shouldly instead of FluentAssertions
        * Add Statistics property to ServiceStatusModel so that checks can add additional information to the health response
        * Add Host to HealthModel and default to MachineName
        * Add enforcement of timeout in Check so that a long running check can't cause the loop to take longer than the cache period causing cache eviction and then health failure


* Cortside.DomainEvent

    * Changes
        * Updated powershell scripts to latest versions from coeus/shoppingcart-api
        * Standardized library build files and resolved code coverage issues
        * Update target framework to net8.0
        * Update all dependency nuget packages
        * Add/Fix build badges
        * Transition to use Shouldly instead of FluentAssertions
        * Add DomainEventCheck as new health check that can report broker errors as well as domain event statistics, with README documentation
        * Add configuration for PublishRetryInterval instead of hard coded 60s value
        * Add new index on status and last modified to outbox (will require migration)
        * Updates to documentation to make configuration more understandable.  Separate Azure Service Bus from RabbitMQ documentation
        * Add support for keyed configuration of receivers and publishers for the benefit of being able to have multiple, i.e. being able to receive from 2 different queues with differing broker configuration
        * Add update-legacyappsettings.ps1 script (in docs folder) to migrate configuration in appsettings.json to new style, old style will be deprecated in future


* Cortside.MockServer

    * Changes
        * Updated powershell scripts to latest versions from coeus/shoppingcart-api
        * Standardized library build files and resolved code coverage issues
        * Update target framework to net8.0
        * Update all dependency nuget packages
        * Add/Fix build badges
        * Transition to use Shouldly instead of FluentAssertions
        * Add support for mocking of Cortside.Authorization policies


* Cortside.RestApiClient

    * Changes
        * Updated powershell scripts to latest versions from coeus/shoppingcart-api
        * Standardized library build files and resolved code coverage issues
        * Update target framework to net8.0
        * Update all dependency nuget packages
        * Add/Fix build badges
        * Transition to use Shouldly instead of FluentAssertions
        * Add property for EnableForwardHeaders to IRestApiClientOptions to optionally configure forwarding headers, defaults to true for backwards compatability



* Cortside.AspNetCore

    * Changes
        * Updated powershell scripts to latest versions from coeus/shoppingcart-api
        * Standardized library build files and resolved code coverage issues
        * Update target framework to net8.0
        * Update all dependency nuget packages
        * Add/Fix build badges
        * Transition to use Shouldly instead of FluentAssertions
        * Add handling of PreconditionFailedResponseException to return 412 in MessageExceptionResponseFilter
        * Add new EF Core interceptor for handling of audit responsibilities, moving functionality from AuditableDatabaseContext, this will allow other custom implemented database context classes to still benefit from audit stamping
        * Broke out some common model builder methods from AuditableDatabaseContext to new ModelBuilderExtensions extension class
        * Add Authorization parameter to swagger model
        * Add definition for CustomSchemaId and CustomOperationIds to swagger definition
        * Add helper extension methods for ToPagedResult and ToListResult on IList<T>
        * Add IConfiguration support for values to have substitutable values anywhere in configuration with IConfiguration extension method ExpandTemplates
        * Conditionally add security to swagger model if identity authority is configured
        * Add support for Cortside.Authorization in AccessControlConfiguration


## Changes

## Migration notes

* ./clean.ps1
* dotnet test src
* ./update-nugetpackages.ps1 -NoVersionLock
* In DomainEventPublisherSettings, change `AppName` to `Service`, `PolicyName` to `Policy`
* In DatabaseContext class, if using, change:
    ```csharp
    SetDateTime(modelBuilder)l
    SetCascadeDelete(modelBuilder);
    ```

    to:

    ```csharp
    modelBuilder.SetDateTime()l
    modelBuilder.SetCascadeDelete();
    ```
* In IntegrationFixture (or whatever name is) class, if not using `services.RegisterInMemoryContext`, add the following with the other Unregister lines:
    ```cscharp
    services.Unregister<IDbContextOptionsConfiguration<DatabaseContext>>();
    ```
* make sure tests pass - `dotnet test src`
* .\add-migration.ps1 -migration OutboxPublisherKey
* Use https://github.com/cortside/cortside.domainevent/blob/develop/docs/update-legacyappsettings.ps1 to change domainevent configuration to unified shape:
    ```json
    "DomainEvent": {
        "Connections": [
          {
            "Protocol": "amqp",
            "Server": "localhost",
            "Username": "admin",
            "Password": "password",
            "Queue": "shoppingcart.queue",
            "Topic": "/exchange/shoppingcart/",
            "Credits": 5,
            "ReceiverHostedService": {
            "Enabled": true,
            "TimedInterval": 60
            },
            "OutboxHostedService": {
            "BatchSize": 5,
            "Enabled": true,
            "Interval": 5,
            "PurgePublished": false
            }
          }
        ]
    }
    ```

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
