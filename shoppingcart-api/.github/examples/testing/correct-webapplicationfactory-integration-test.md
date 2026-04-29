# Correct: WebApplicationFactory Integration Test

```csharp
public class OrdersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetOrder_ReturnsOk()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/orders/seed-order-id");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```
