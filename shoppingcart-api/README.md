# Acme.ShoppingCart

This project was created from the cortside-api template by doing the following:

```text
dotnet new --install cortside.templates
dotnet new cortside-api -n Acme.ShoppingCart
```

The template repo can be found at https://github.com/cortside/cortside.templates.

[Service Diagram](docs/PrequalificationServiceDiagram.md)

## Pre-Requisites

- .NET SDK equal or greater to version number in `/deploy/ps/version.ps1`
- Latest Visual Studio
- NuGet Package Sources:
  - http://oa-utility:81/nuget/aggregated/
  - http://oa-utility:81/nuget/develop (Call it: Cortside ProGet)

## Visual Studio Extensions

Here are the Visual Studio Extensions we use for this project:

- EditorConfig Language Service
- Format document on Save
- SonarLint for Visual Studio
- Extension Manager
- Code Cleanup On Save

# First Time To Run

Before you run this for the first time you will need to run powershell script `.\build.ps1`

This will create the `build.json` file in:
\cortside.prequalification\src\Cortside.Prequalification.WebApi\build.json

## Create database locally

Run powershell script `.\update-database.ps1 -CreateDatabase`

## Update existing local database

Run powershell script `.\update-database.ps1`

## Seed local database

Use SqlServer Management Studio (ssms) to import data from `Prequalification` db in `SqlTestOnlineApp` server.

- from localhost, right click on `Prequalification` db, then go to `Tasks`, then `Import Data`
- follow the wizard to select source and destination, using integrated security. Ask in #pp slack channel if help is needed.
- target all tables other than `audit.*`

## Updating the database schema through migrations

### Add a new migration

- In Cortside.Prequalifiation.Data, make changes to the database models to match the desired schema.
- To generate a new migration, run: `add-migration.ps1` and supply a name for the migration when prompted
  - also runs `Generate-Sql.ps1` and `Generate-SqlTriggers.ps1`
- If needed, edit and add to the generated C# migration file.
- If needed, run the `Generate-Sql.ps1` script to generate the SQL file.
- If needed, run the `Generate-SqlTriggers.ps1` script.
- Run the `update-database.ps1` script to update your local instance.

### Edit an existing migration

The way EntityFramework migrations work, particularly how we use them, is that editing a migration **after** it has already run in a database, will accomplish nothing. If you read the generated sql files that are run by `update-database.ps1`, you will see why. The deployment process does not ever run something like `dotnet ef down --target-migration some_previous_migration`.

That means editing existing migrations can really only be done sensibly in a branch that has not yet been merged **and preferably not yet deployed to any integrated environment like dev or stage** (or there will likely be future issues to deal with there). But know that you will need to manually delete the record in your local database's `__EFMigrationsHistory` table to run the updated script against your local db, one way or another. And undo what it did, one way or another. Deleting your local db and running `update-database.ps1 -CreateDatabase` is a viable option.

In other words, **if your branch has not yet merged AND has not yet deployed to dev/test/stage/uat/etc** then:

- delete your **local** db`*`
- delete the generated sql migration file
- update the cs migration _or_ delete the existing cs migration and add a fresh one
- repeat the remaining steps for adding a migration

`*`**NOTE:** This is one way and it's the big hammer way. Combining with the details above of migrations work, we can also rollback to a previous version, and then undo the migration changes (files, entities, etc).

## Deployment of Database changes

Database changes are deployed to shared environments when Octopus runs `update-database.ps1` against the database for the given environment being deployed to.

## Running the application locally

- `run.ps1` is a convenience script, if debugging in VisualStudio is not needed
- you may need to run `update-database.ps1` to update your local database (do not run against SqlTestOnlineApp, as deployments should do that)
- you may need to run rabbitmq in docker locally, when working with domain event message publication and consumption
  - pre-reqs: docker-desktop installed and running locally
  - clone http://bitbucket:7990/projects/SYS/repos/systemtest/browse
  - run `start-rabbitmq.ps1` - this will start a container and configure queues and subscriptions per the config in the cloned repo
    - if working on a new domain event with new configuration, add it in this repo (and create a pull request) in addition to the Azure ServiceBus repo
    - admin UI can be accessed at http://localhost:15672/ with admin/password as credentials

## Pre-Requisites

* .NET 6.0
* Visual Studio 2022

## Feature Overview

For a more detailed understanding of the features and their relation in the code see [Features.md](Features.md).

* .net core api
* IdentityServer/OpenID Connect authentication
* Permission based authorization with PolicyServer
* Health checks
  * /health endpoint
  * background health service
  * custom check
* AMQP message publication
  * using outbox pattern for resilience
* AMQP message consumption
* OpenAPI
* miniprofiler
* Logging
* ApplicationInsights


## Other

* Distributed locks using [DistributedLock](https://github.com/madelson/DistributedLock)
    * Distributed locks can be help in coordinating work between multiple instances of a service or potentially across services.  The [OrderStateChangedHandler](src/Acme.ShoppingCart.DomainEvent/OrderStateChangedHandler.cs) creates a lock based on the orderResourceId to ensure that multiple messages are not interacting with the same resource at the same time.  The The nuget package provides differing lock providers based on case, i.e. the service may use the database or redis instance that would be common to all of the instances of the service running.  The integration tests use file provider to make it easer for tests to be run in a build environment with minimal dependencies.
* Timed background service
    * It is not uncommon to have periodic actions take place.  These can be handled in one of two ways, either through an external cron process that publishes an amqp message to "do the work" or by a background process in the service.  [ExampleHostedService](src/Acme.ShoppingCart.Hosting/ExampleHostedService.cs) is an example that implements a background process.  This example doesn't do anything other than log that it's doing something, but data cleanup or delayed processing could happen here.  It might be a good ideal to make use of a distributed lock to make sure that multiple instances aren't doing the same work at the same time.
* Concrete model mapping
    * [AutoMapper](https://github.com/AutoMapper/AutoMapper) can be a decent choice for smaller projects but has limitations in debugging and class/member reference tracking in code.  The mappings in this project were created by hand in conjuction with the free version of a visual studio [plugin](https://mappinggenerator.net/) to enable faster creation.  For an article that talks about issues with AutoMapper, see it [here](https://cezarypiatek.github.io/post/why-i-dont-use-automapper/).  It's also worthwhile to read the author's intended [usage guidelines](https://jimmybogard.com/automapper-usage-guidelines/).
* Api specific exposed models
    * So that the exact exposure of the api can be deliberately controlled and can differ from underlying implementation, specific request and response models exist in the WebApi project to expose.  Use of the mappers above should easy the coercion from one type to another in an explicit way for api consumers.  
* Facades
    * This is a layer of the service that provides an abstraction to simplify the interactions that can take place with domain services.  This is also that layer that should be defining the start and stop of a unit of work as well as any transactional isolation level needs.  See [OrderFacade](src/Acme.ShoppingCart.Facade/OrderFacade.cs) for an example of coordinating work between multiple domain services will retaining control of the unit of work.  See [CustomerFacade](src/Acme.ShoppingCart.Facade/CustomerFacade.cs) for an example of setting the isolation level for a unit of work while performing a search.  This layer should be responsible for translating from entities to dtos.
* UnitOfWork
    * started when injected
    * used to control the scope of the over all transaction.  it's implied by single calls to it's SaveChangesAsync method
    * can be used to begin an explicit transaction

* copy of files
  * don't specify copy always unless really needed, it adds additional build time with dependencies

* entity class as domain logic class

* bowdlerizer

* integration tests with testserver

## todo

* ListResult example
* example of sending email that uses db identity column and is part of atomic transaction -- need for strategy?
* delete/remove example in repository?
* commit postman collection
* get readme stuff from pq -- things like migrations
* date only in request/response
* datetime vs datetimeoffset