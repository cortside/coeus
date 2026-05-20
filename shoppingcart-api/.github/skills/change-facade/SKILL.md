---
name: change-facade
description: Add or modify facade orchestration while preserving layering boundaries.
argument-hint: Facade method to add or change, involved domain services, and expected DTO output
user-invocable: true
disable-model-invocation: false
---
# Change Facade Skill

## When To Use
Use when orchestration logic across multiple domain services needs to be added or changed, or when transaction boundaries need to be established or modified.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Architecture Layering Governance](../../instructions/architecture-layering-governance.instructions.md)
- [Mapping Governance](../../instructions/mapping-governance.instructions.md)

## Step-By-Step Procedure
1. Define the facade method signature with async Task<TResult>, input command/query model, and CancellationToken.
2. If the workflow spans multiple domain service calls that must succeed together, wrap in a unit-of-work or transaction scope.
3. Call domain services in the correct order. Do not embed business rule logic directly in the facade — delegate to domain services.
4. Map domain results to DTOs explicitly at the facade boundary before returning to the controller or handler.
5. Do not include HTTP status codes, HttpContext, or message broker message types in the facade.
6. Handle and propagate domain exceptions — do not swallow them or remap them to HTTP concerns.
7. Add or update unit tests covering:
   - Happy-path orchestration result.
   - Domain service exception propagation.
   - Transaction rollback behavior where applicable.
   - Branching paths for conditional domain service calls.
8. If the facade change affects controller or handler callers, update their mappings accordingly.

## Acceptance Criteria
- Facade orchestrates domain services without owning business rules.
- No HTTP or broker concerns present in the facade.
- Domain-to-DTO mapping is explicit at the facade boundary.
- Unit tests cover orchestration paths and exception propagation.

## Edge Cases
- Partial failure in multi-service orchestration: ensure transaction rollback is tested.
- Facades that call external REST clients via domain services: mock the external client in unit tests.
- Large result sets: consider pagination contracts at the facade-to-DTO boundary.

## Stop Conditions
- Stop after orchestration is correctly layered, mapping is explicit, and tests pass.
