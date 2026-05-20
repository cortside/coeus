# Incorrect: Controller To Repository

**Why this is incorrect:**
The controller accesses DbContext directly, bypassing the facade, domain service, and repository layers entirely. This puts persistence logic and implicit business behavior (no validation, no domain rules enforced) directly in the transport layer.

```csharp
[HttpPost]
public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
{
    var entity = new OrderEntity { CustomerId = request.CustomerId };
    _dbContext.Orders.Add(entity); // Direct persistence from controller is prohibited
    await _dbContext.SaveChangesAsync();
    return Ok(entity.Id);
}
```

**What to do instead:**
Delegate to a facade. Let the facade coordinate domain services. Let domain services enforce rules and call repositories. Let repositories interact with DbContext.
