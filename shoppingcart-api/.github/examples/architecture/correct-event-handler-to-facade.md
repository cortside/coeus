# Correct: Event Handler To Facade

```csharp
public async Task HandleAsync(CustomerStateChangedMessage message, CancellationToken ct)
{
    if (message is null) throw new ArgumentNullException(nameof(message));
    var dto = new CustomerStateChangedDto(message.CustomerId, message.NewState);
    await _customerFacade.ProcessStateChangeAsync(dto, ct);
}
```
