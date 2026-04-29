---
description: "Entity Framework Core safety and performance governance."
name: "EF Core Governance"
applyTo: "src/Acme.ShoppingCart.Data/**/*.cs"
---
# EF Core Governance

## Boundary Rules
- DbContext usage belongs in repository or infrastructure boundaries unless existing architecture explicitly differs.
- Do not bypass repository abstractions.

## Query Rules
- Prefer async APIs and pass `CancellationToken` where supported.
- Use `AsNoTracking()` for read-only queries.
- Keep tracking queries when entity updates are required.
- Use includes and projections intentionally.
- Avoid N+1 and accidental client-side evaluation.

## Contract Rules
- Do not expose `IQueryable` outside allowed query boundaries unless existing architecture explicitly permits it.
- Preserve concurrency, audit, and soft-delete conventions when present.
- Do not create migrations unless explicitly requested.
