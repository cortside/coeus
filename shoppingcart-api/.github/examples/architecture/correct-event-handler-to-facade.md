# Correct: Event Handler To Facade

**Why this is correct:**
The handler validates the message, maps the broker contract to an internal DTO, and delegates to the facade. No business logic lives in the handler.

```csharp
public async Task HandleAsync(CustomerStateChangedMessage message, CancellationToken ct)
{
    if (message is null) throw new ArgumentNullException(nameof(message));
    var dto = new CustomerStateChangedDto(message.CustomerId, message.NewState);
    await _customerFacade.ProcessStateChangeAsync(dto, ct);
}
```

**Key governance points:**
- Null guard on the message contract before any work.
- Explicit mapping from broker message type to internal DTO at the handler boundary.
- Facade is the only business workflow entry point — no domain service or repository calls directly from the handler.
