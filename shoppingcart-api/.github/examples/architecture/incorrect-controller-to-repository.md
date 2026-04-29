# Incorrect: Controller To Repository

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
