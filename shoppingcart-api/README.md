# Acme.ShoppingCart

This project was created from the cortside-api template by doing the following:

```text
dotnet new --install cortside.templates
dotnet new cortside-api -n Acme.ShoppingCart
```

The template repo can be found at https://github.com/cortside/cortside.templates.

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

* bowdlerizer

* integration tests with testserver

## todo

* ListResult example
