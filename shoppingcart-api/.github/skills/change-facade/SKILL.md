---
name: change-facade
description: Add or modify facade orchestration while preserving layering boundaries.
argument-hint: Facade workflow changes and involved services
user-invocable: true
disable-model-invocation: false
---
# Change Facade Skill

## Required Checks
- Facade handles orchestration and transaction boundaries.
- Facade delegates business rules appropriately to domain services/entities.
- Facade maps domain outputs to DTOs explicitly.
- Unit tests cover orchestration and branching behavior.

## Prohibited Actions
- HTTP or message broker concerns in facades.
- AutoMapper usage.
