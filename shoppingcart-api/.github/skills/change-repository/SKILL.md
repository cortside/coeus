---
name: change-repository
description: Add or modify repository behavior with EF Core safety and performance controls.
argument-hint: Repository method change and query expectations
user-invocable: true
disable-model-invocation: false
---
# Change Repository Skill

## Required Checks
- Query and persistence logic remain in repository layer.
- Async EF APIs and cancellation are used.
- AsNoTracking is used for read-only queries.
- Query shape avoids N+1 and client-eval pitfalls.
- Tests cover key query/persistence behavior.

## Prohibited Actions
- Business logic in repositories.
- Upward dependencies to controllers, handlers, facades, or domain services.
