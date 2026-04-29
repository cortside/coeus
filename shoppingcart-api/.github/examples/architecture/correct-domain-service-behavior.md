# Correct: Domain Service Behavior

```csharp
public async Task<Order> ApplyDiscountAsync(Guid orderId, decimal percent, CancellationToken ct)
{
    var order = await _orderRepository.GetByIdAsync(orderId, ct) ?? throw new OrderNotFoundException(orderId);
    order.ApplyDiscount(percent);
    await _orderRepository.SaveAsync(order, ct);
    return order;
}
```
