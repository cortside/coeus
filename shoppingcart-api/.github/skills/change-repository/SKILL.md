---
name: change-repository
description: Add or modify repository behavior with EF Core safety and performance controls.
argument-hint: Repository method purpose, read vs write intent, related EF entities, and query shape
user-invocable: true
disable-model-invocation: false
---
# Change Repository Skill

## When To Use
Use when adding or changing database query or persistence behavior, EF Core query shapes, or domain-entity mapping in the repository layer.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [EF Core Governance](../../instructions/efcore-governance.instructions.md)
- [Architecture Layering Governance](../../instructions/architecture-layering-governance.instructions.md)

## Step-By-Step Procedure
1. Determine whether the operation is read-only or write. Apply AsNoTracking() for read-only queries.
2. Write the EF Core query using async APIs (ToListAsync, FirstOrDefaultAsync, SingleOrDefaultAsync) and always pass CancellationToken.
3. Use explicit Include() or Select() projections. Do not rely on lazy loading.
4. Review the query for N+1: ensure related entities are loaded in a single round-trip via Include or a join projection.
5. Do not expose IQueryable outside the repository method. Return domain entities or value types.
6. Map EF entities to domain entities explicitly within the repository method. Do not return EF entity types to callers.
7. For write operations: retrieve the entity, mutate via domain entity methods, then persist. Do not call SaveChanges without intention.
8. Do not create or modify migrations unless explicitly requested.
9. Add unit tests using an in-memory or sqlite EF provider (or fakes consistent with existing test patterns) covering:
   - Records found path.
   - Records not found path (null/empty result).
   - Write/persist confirmation.
   - Query filter correctness.

## Acceptance Criteria
- All EF calls are async with CancellationToken.
- Read-only queries use AsNoTracking().
- No IQueryable returned beyond the repository.
- EF entities are not exposed to callers — domain entities returned.
- No business logic present in the repository.
- Tests cover key query shapes and persistence behavior.

## Edge Cases
- Soft-delete patterns: apply global query filters or explicit IsDeleted checks per existing conventions.
- Concurrency tokens: preserve RowVersion or ConcurrencyStamp fields when persisting.
- Bulk operations: prefer EF ExecuteUpdateAsync/ExecuteDeleteAsync for large sets per .NET 10 conventions.

## Stop Conditions
- Stop after query correctness is confirmed, EF safety checks pass, and tests pass.
