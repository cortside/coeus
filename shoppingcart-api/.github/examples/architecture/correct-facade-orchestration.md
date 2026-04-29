# Correct: Facade Orchestration

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
