---
description: "Service-tier architecture boundaries and allowed dependencies."
name: "Architecture Layering Governance"
applyTo: "src/**/*.cs"
---
# Architecture Layering Governance

## Required Flows
- HTTP Request -> Controller -> Facade -> Domain Service -> Repository -> Database.
- Message Queue -> Event Handler -> Facade -> Domain Service -> Repository -> Database.

## Tier Responsibilities
- Controllers: transport, validation orchestration, shape mapping.
- Event Handlers: message contract handling and mapping.
- Facades: cross-service orchestration and transaction scope.
- Domain Services: domain workflow and business rules across entities.
- Repositories: data access and persistence mapping.
- Domain Entities: invariants and aggregate behavior.

## Dependency Boundaries
- Controllers and event handlers call facades, not repositories.
- Facades call domain services and approved abstractions.
- Domain services call repositories and domain entities.
- Repositories do not call facades, controllers, or handlers.
