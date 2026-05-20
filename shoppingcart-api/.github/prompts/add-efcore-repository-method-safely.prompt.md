---
description: "Add or modify an EF Core repository method safely."
name: "Add EF Core Repository Method Safely"
argument-hint: "Repository method purpose, query shape, and read vs write intent"
---
Add or update a repository method with EF Core governance.

1. Use async EF APIs with CancellationToken throughout.
2. Apply AsNoTracking() on read-only queries.
3. Use explicit includes or projections — no lazy loading.
4. Avoid IQueryable exposure beyond repository boundary.
5. Check for N+1 patterns before finalizing query shape.
6. Do not create migrations unless explicitly requested.
7. Add or update repository unit tests for key query and persistence behavior.

For a detailed step-by-step playbook use the `change-repository` skill.
