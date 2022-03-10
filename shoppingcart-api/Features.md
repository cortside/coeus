# Features

This template has been created to demonstrate sample implementation of key features of an expected microservice oriented api.  The template is based around a shopping cart or order, that gets pricing information for another catalog api serivce. The actual domain functionality is implemented only to the point of being able to illusatrate concepts.

## Feature Overview

You can find documentation that points out the expectations or needs of a microservices and related architectural documentation [here](https://github.com/cortside/guidelines/tree/master/docs/architecture).

### Synchronous communication

[Synchronous communication](https://github.com/cortside/guidelines/blob/master/docs/architecture/Microservices.md#synchronous-communication) between services is handled by use of HTTP requests and responses responses.  The CatalogClient is an implementation of this, making use of a [RestSharp](https://github.com/restsharp/RestSharp) based [base client](https://github.com/cortside/cortside.restsharpclient) that itself handles authentication, logging of operations, correlation, serialization, caching, and error handling.

### Asynchronous messaging

[Asynchronous messaging](https://github.com/cortside/guidelines/blob/master/docs/architecture/Microservices.md#asynchronous-messaging) is used to communicate changes and events between services by use of AMQP messages.

Each service will subscribe to the message types that it's interested in.  [`RecieverHostedService`](https://github.com/cortside/cortside.domainevent), which uses `DomainEventReceiver`, handles listening for any inbound messages.  The receiver will use the service provider to instantiate an instance of a message handler, i.e. `OrderStateChangedHandler`.  The handler will respond with success or failure, or whether to retry the message or let the message broker redeliver the message to another consumer.

Each service may publish messages for events that occur for others services to subscribe to.  A publisher implementation can be chosen that will publish directly to the broker ([DomainEventPublisher](https://github.com/cortside/cortside.domainevent)) or through the use of an "outbox" ([DomainEventOutboxPublisher](https://github.com/cortside/cortside.domainevent)).  An advantage of using an outbox is that the publish of a message can participate in a transaction with the data store.  The `OutboxHostedService` will poll the outbox table for messages to publish to the message broker and guarantee successful publishing at least once.

### Authentication

Most services will require authentication for part or all of the exposed api endpoints.  This template uses [IdentityServer 4](https://github.com/IdentityServer/IdentityServer4) for an identity provider.  The [IdentityServerInstaller](src/Acme.ShoppingCart.WebApi/Installers/IdentityServerInstaller.cs) installer sets up the middleware to handle authentication that enforces `Authorize` attributes applied to controllers such as [CustomerController](src/Acme.ShoppingCart.WebApi/Controllers/CustomerController.cs).

### Authorization

It is common for services that require authentication to also require authorization.  Authorization is handled by the use of [PolicyServer](https://solliance.net/products/policyserver), which provides both role based and permission based authorization.  The [IdentityServerInstaller](src/Acme.ShoppingCart.WebApi/Installers/IdentityServerInstaller.cs) installer sets up the middleware to handle authorization that enforces a permission specified in the `Authorize` attributes applied to controllers such as [CustomerController](src/Acme.ShoppingCart.WebApi/Controllers/CustomerController.cs).

### Observability

In order to be able to understand and monitor the execution, performance and health of a service, this template implements the following observability aspects:

* Logging
  * Logging, specifically structured logging, is handled by [Serilog](https://github.com/serilog/serilog) and is configured through a combination of values in [appsettings.json](src/Acme.ShoppingCart.WebApi/appsettings.json) and code in [Program.cs](src/Acme.ShoppingCart.WebApi/Program.cs).  Serilog supports output to multiple sinks, of which console, file, [Seq](https://datalust.co/seq) or an aggregated repository like [Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview).  
* Audit Logging
  * Entities that implement [AuditableEntity](src/Acme.ShoppingCart.Domain/AuditableEntity.cs) will automatically have creation and last modified audit stamps set by [AuditableDatabaseContext](src/Acme.ShoppingCart.Data/AuditableDatabaseContext.cs).  The properties/columns in `AuditableEntity` are also used by generated triggers to keep a full audit log of changes to entities, i.e. [trCustomer.trigger.sql](src/sql/trigger/trCustomer.trigger.sql).
* Distributed tracing
  * To aide in correlation of log entries and events across services, a [correlationId](https://github.com/cortside/cortside.common/tree/develop/src/Cortside.Common.Correlation) is either pulled from the request context or created and set for the service to reference.  `CorrelationMiddleware` is added to the request pipeline in [Startup.cs](src/Acme.ShoppingCart.WebApi/Startup.cs).  The correlationId is automatically passed between services when deriving from the [rest base client](https://github.com/cortside/cortside.restsharpclient) or when publishing messages using [DomainEventPublisher](https://github.com/cortside/cortside.domainevent).  In structured logging and with aggregated log service, it becomes possible to search by the correlationId property across services.
* Health checks
  * Each service should be responsible for determining and exposing it's own health along with the health of any dependencies.  This is accomplished by exposing an endpoint as well as by [HealthCheckHostedService](https://github.com/cortside/cortside.health) which is registered in [HealthInstaller](src/Acme.ShoppingCart.BootStrap/Installer/HealthInstaller.cs) installer as a background service to perform the individual checks that determine the overall health.  Each check can be configured via [appsettings.json](src/Acme.ShoppingCart.WebApi/appsettings.json).  A custom check example can be found in [ExampleCheck.cs](src/Acme.ShoppingCart.Health/ExampleCheck.cs).
  * Health information is pushed to Application Insights via the [ApplicationInsightsRecorder.cs](https://github.com/cortside/cortside.health/blob/develop/src/Cortside.Health/Recorders/ApplicationInsightsRecorder.cs).
* Application metrics
  * The application performance management (APM) [Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview) is setup in [Startup.cs](src/Acme.ShoppingCart.WebApi/Startup.cs).  The Applications Insights portal exposes information like performance and failure metrics, exceptions, dependencies and other metrics.

### OpenAPI

OpenAPI is a standard to describe REST APIs and it allows you to declare your API security method, endpoints, request/response data, and HTTP status messages.  The OpenAPI document is generated by use of [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) and is setup in [SwaggerInstaller](src/Acme.ShoppingCart.WebApi/Installers/SwaggerInstaller.cs) installer.  The installer sets up multiple [document and operation filters](src/Acme.ShoppingCart.WebApi/Filters) that augment the OpenAPI document for version, authentication and authorization information.  Controllers, request and response models should all be attributed with comments that will be used in the generated OpenAPI document.  Controllers should also attributed with `ProducesResponseType` attributes to denote the response model definition, i.e. [CustomerController](src/Acme.ShoppingCart.WebApi/Controllers/CustomerController.cs).
