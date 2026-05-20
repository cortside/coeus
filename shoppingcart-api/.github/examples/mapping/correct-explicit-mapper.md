# Correct: Explicit Mapper

**Why this is correct:**
Mapping is done property-by-property in a readable static function. Every field assignment is visible and reviewable. The function is testable in isolation.

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

**Key governance points:**
- No AutoMapper, Mapster, or reflection — every property is explicit.
- Enum converted to string explicitly — no implicit coercion.
- Function is static and independently testable.
- Lives at the correct layer boundary (e.g. this belongs in the facade since it maps domain Order to service DTO).
