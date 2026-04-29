# Correct: Controller To Facade

```csharp
[HttpPost]
public async Task<IActionResult> CreateOrder(CreateOrderRequest request, CancellationToken ct)
{
    if (!ModelState.IsValid) return ValidationProblem(ModelState);
    var command = new CreateOrderCommand(request.CustomerId, request.Items);
    var result = await _orderFacade.CreateOrderAsync(command, ct);
    return CreatedAtAction(nameof(GetOrder), new { id = result.OrderId }, new CreateOrderResponse(result.OrderId));
}
```
