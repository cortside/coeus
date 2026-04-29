# Correct: Unit Test Structure

```csharp
[Fact]
public async Task CreateOrderAsync_ReturnsCreatedOrder_WhenInputIsValid()
{
    // Arrange
    var facade = BuildFacade();
    var command = new CreateOrderCommand(CustomerId, Items);

    // Act
    var result = await facade.CreateOrderAsync(command, CancellationToken.None);

    // Assert
    result.OrderId.Should().NotBeEmpty();
}
```
