# Correct: Controller To Facade

**Why this is correct:**
The controller binds the request, validates inputs, maps to an internal command, delegates all workflow to the facade, and maps the result to a response DTO. No business logic lives in the controller action.

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

**Key governance points:**
- Validation checked before any logic.
- Request DTO mapped to command model at the controller boundary.
- Facade call carries CancellationToken.
- Result mapped to response DTO before returning — facade result type never leaks to the HTTP response directly.
