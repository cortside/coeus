# Correct: Unit Test Structure

**Why this is correct:**
The test follows Arrange/Act/Assert clearly, uses a descriptive name that encodes the scenario, and asserts only on observable behavior (the returned OrderId). No internal state or implementation sequence is asserted.

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

**Key governance points:**
- Test method name follows `MethodName_ExpectedResult_WhenCondition` pattern.
- `BuildFacade()` helper creates the SUT with mocked/faked dependencies — not real infrastructure.
- Assertion is on the observable result, not on internal method calls.
- Companion tests should exist for: validation failure, dependency exception, and null/empty inputs.
