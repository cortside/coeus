# Correct: Explicit Mapper

```csharp
public static OrderDto ToDto(Order order)
{
    return new OrderDto(
        Id: order.Id,
        CustomerId: order.CustomerId,
        Total: order.Total,
        State: order.State.ToString());
}
```
