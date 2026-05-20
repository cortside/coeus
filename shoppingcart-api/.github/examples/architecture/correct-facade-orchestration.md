# Correct: Facade Orchestration

**Why this is correct:**
The facade coordinates multiple domain service calls inside a unit-of-work transaction. It owns the transaction boundary and maps domain results to a DTO. It does not implement business rules — it delegates to domain services.

```csharp
public async Task<CheckoutResultDto> CheckoutAsync(CheckoutCommand command, CancellationToken ct)
{
    using var tx = await _unitOfWork.BeginTransactionAsync(ct);
    var order = await _orderDomainService.CreateOrderAsync(command, ct);
    await _paymentDomainService.AuthorizeAsync(order, ct);
    await _unitOfWork.CommitAsync(ct);
    return new CheckoutResultDto(order.Id, order.State);
}
```

**Key governance points:**
- Transaction boundary owned at the facade — not in a domain service or controller.
- Business rules for order creation and payment authorization live in the respective domain services.
- Domain result mapped to DTO explicitly before returning to caller.
- No HTTP concerns (no IActionResult, no HttpContext) present.
