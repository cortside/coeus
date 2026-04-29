# Correct: Repository Behavior

```csharp
public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
{
    var entity = await _dbContext.Orders.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
    return entity is null ? null : MapToDomain(entity);
}
```
