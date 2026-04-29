---
name: change-domain-service
description: Add or modify domain service behavior with business-rule ownership preserved.
argument-hint: Domain service method and business rule changes
user-invocable: true
disable-model-invocation: false
---
# Change Domain Service Skill

## Required Checks
- Domain rules stay in domain layer.
- Repository abstractions are used for persistence access.
- Transport concerns are excluded.
- Async and cancellation patterns are respected.
- Unit tests updated for rule changes.

## Prohibited Actions
- Controller or facade-only concerns in domain services.
- Direct git write operations.
