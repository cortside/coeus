# Correct: Domain Service Behavior

**Why this is correct:**
The domain service retrieves the entity through a repository abstraction, delegates the business operation to the entity itself, and persists through the repository. No HTTP concerns, no controller DTOs, no direct DbContext.

```csharp
public async Task<Order> ApplyDiscountAsync(Guid orderId, decimal percent, CancellationToken ct)
{
    var order = await _orderRepository.GetByIdAsync(orderId, ct) ?? throw new OrderNotFoundException(orderId);
    order.ApplyDiscount(percent);
    await _orderRepository.SaveAsync(order, ct);
    return order;
}
```

**Key governance points:**
- Entity retrieved through repository abstraction — no DbContext reference.
- Business rule (ApplyDiscount) lives on the domain entity, not inlined here.
- Domain exception thrown for not-found — not an HTTP 404 directly.
- Domain entity returned to caller (facade) — not a DTO.
