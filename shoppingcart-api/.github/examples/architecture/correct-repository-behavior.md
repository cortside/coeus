# Correct: Repository Behavior

**Why this is correct:**
The repository uses AsNoTracking() for a read-only query, passes CancellationToken, and maps the EF entity to a domain entity before returning it. The caller never sees the EF entity type.

```csharp
public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
{
    var entity = await _dbContext.Orders.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
    return entity is null ? null : MapToDomain(entity);
}
```

**Key governance points:**
- AsNoTracking() applied because this is a read-only query — EF will not track changes.
- CancellationToken passed to the EF async API.
- MapToDomain performs explicit mapping — the repository does not expose EF entity types to callers.
- Returns null cleanly for not-found — does not throw here; callers decide how to handle absence.
