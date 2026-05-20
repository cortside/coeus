---
name: change-domain-service
description: Add or modify domain service behavior with business-rule ownership preserved.
argument-hint: Domain service name, method to add or change, and business rule to enforce
user-invocable: true
disable-model-invocation: false
---
# Change Domain Service Skill

## When To Use
Use when business logic that spans multiple entities or aggregates needs to be added, changed, or corrected in a domain service.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Architecture Layering Governance](../../instructions/architecture-layering-governance.instructions.md)
- [.NET 10 C# Standards](../../instructions/dotnet10-csharp-standards.instructions.md)

## Step-By-Step Procedure
1. Identify which domain service owns the rule. If no existing service fits, evaluate whether a new one is warranted.
2. Retrieve domain entities through repository abstractions — never directly via DbContext.
3. Implement business logic on the domain entity where the rule belongs to a single aggregate; implement cross-entity coordination in the domain service.
4. Use async/await with CancellationToken on all I/O calls.
5. Raise or throw domain-specific exceptions consistent with existing project exception patterns.
6. Do not include HTTP status codes, controller DTOs, or message broker concerns.
7. Update or add unit tests covering:
   - Rule enforcement (happy path).
   - Rule violation (exception or failure path).
   - Entity not found path.
   - Any concurrency or state transition edge cases.
8. If the rule change affects the facade, update the facade delegation accordingly.

## Acceptance Criteria
- Domain rule is implemented and enforced in the domain layer.
- No transport-layer concern (HTTP/broker) present in the domain service.
- Repository abstractions used — no direct DbContext.
- Unit tests cover rule enforcement and violation paths.

## Edge Cases
- Rules that require reading multiple aggregates: keep coordination in the domain service, not the facade.
- Optimistic concurrency conflicts: propagate as domain exceptions; let the facade or handler decide retry strategy.
- External service calls (e.g. CatalogApi): use injected client abstractions; mock in unit tests.

## Stop Conditions
- Stop after domain rule is correctly placed, tested, and boundary compliance is confirmed.
