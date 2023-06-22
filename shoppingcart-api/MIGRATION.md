# Migration notes and instructions

## 2023.06 - API

### New

* Example of EF domain entity comments to be used for data dictionary
	* New powershell script generate-datadictionary.ps1 to generate export of table and column metadata that includes class and property comments
	* new sql vwDataDictionary view
* From Cortside.AspNetCore libraries
	* New IServiceCollection extension method with overrides for AddDatabaseContext<TInterface,TImplementation>()
	* ```csharp
      services.AddDatabaseContext<IDatabaseContext, DatabaseContext>(Configuration);
      ```
* New IServiceCollection extension for setting up domainevent receiver and publisher
	* ```csharp
      services.AddDomainEventReceiver(o => {
                o.UseConfiguration(Configuration);
                o.AddHandler<OrderStateChangedEvent, OrderStateChangedHandler>();
            });
      ```
	* ```csharp
      services.AddDomainEventOutboxPublisher<DatabaseContext>(Configuration);
      ```
* New exention method to register encryption service
	* ```csharp
      services.AddEncryptionService(Configuration["Encryption:Secret"]);
      ```
* New extension method for AddRestApiClient
	* ```csharp
      services.AddRestApiClient<ICatalogClient, CatalogClient, CatalogClientConfiguration>(configuration, "CatalogApi");
      ```
* Addition of AuditableDatabaseContext.OnBeforeSaveChangesAsync to allow additional logic before entities are actually saved
* Use of AuditableEntityDto and SubjectDto from Cortside.AspNetCore.Common.Dtos
* Utility class for setting up common Newtonsoft default settings
	* ```csharp
      JsonConvert.DefaultSettings = JsonNetUtility.GlobalDefaultSettings;
      ```
* Use of AuditableEntityModel from Cortside.AspNetCore.Common.Models
* New extension method override for swagger
	* ```csharp
      services.AddSwagger(Configuration, "Acme.ShoppingCart API", "Acme.ShoppingCart API", new[] { "v1", "v2" });
      ```
* Ability to build project inside docker container and output docker image for running built project
	* build-dockerimages.ps1

### Breaking Changes

* OpenIDConnectAuthenticator needs IHttpContextAccessor to be able to get access to request token in order to be able to check for delegation
* OutboxPublisher namespace change to Cortside.Domainevent.EntityFramework from Cortside.DomainEvent 

### Changes

* Introduction of repository.json for repository variable values
	* This allows powershell script to reference repository.json for things that vary and to be common/consistent across repositories making it easier to update these scripts.
	* repository.json now includes the version number, deprecating the need for src/version.json
* Update to all database related powershell scripts to reference repository.json
	* Updates also allow for use of env vars to set server name, user and password; allowing for sql express, sql developer, remote server, local or remote docker container
* Update to generate-sqltriggers.ps1 to no longer rely on stored procs to generate triggers
* Fixed incorrect namespaces and code formatting
* Better example of domainevent handling with OrderStateChangedHandler

## 2023.06 - Web

* continued work in progress for initial full examples

## 2023.01 - API

### New
* Addition of .gitattributes with more explicit handling set

### Breaking Changes

* Changes to ApplicationInsights registration
	* Serilog:WriteTo -- add connectionString property as emtpy string to ApplicationInsights element
	* ApplicationInsights section should replace InstrumentationKey with ConnectionString
* Use of Cortside.Health ServiceCollections extentions for health registration
	* https://github.com/cortside/coeus/tree/develop/shoppingcart-api#database-location

### Changes

* Updates to database related scripts for better handling different database editions and location.  See notes here:
	* https://github.com/cortside/coeus/tree/develop/shoppingcart-api#database-location
* Updates to how Generate-SqlTriggers.ps1 works to better support other sql editions as well as remote server.  Replaces use of stored procedures that wrote out generated files by doing text generation in powershell.
* More implementation of business logic examples in domain entities, domain services and facades
* Integration tests resolve Json serializer settings from services for use in serializing as opposed to relying on global serializer settings.  This is to resolve conflicts in latest test libraries from microsoft that don't handle custom serializer classes from newtonsoft.

## 2023.01 - Web

* continued work in progress for initial full examples
 