---
description: "Add or modify an EF Core repository method safely."
name: "Add EF Core Repository Method Safely"
argument-hint: "Repository behavior and expected query shape"
---
Add or update a repository method with EF Core governance.

Required checks:
- Async EF API usage.
- CancellationToken support where appropriate.
- AsNoTracking for read-only queries.
- No IQueryable leakage unless architecture permits.
- Intentional includes/projections.
- Avoid N+1 and client evaluation surprises.

Do not create migrations unless explicitly requested.
Add or update repository tests as practical.
