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

* .net core api
* IdentityServer/OpenID Connect authentication
* Policy base authorization
* Health checks
  * /health endpoint
  * background health service
  * custom check
* AMQP message publication
  * using outbox pattern for resilience
* AMQP message consumption
* distributed lock
* Timed background service
* OpenAPI
* miniprofiler
* Logging
* ApplicationInsights
* integration tests with testserver

For a more detailed understanding of the features and their relation in the code see [this feature list](Features.md).
