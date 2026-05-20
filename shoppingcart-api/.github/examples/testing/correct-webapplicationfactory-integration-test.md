# Correct: WebApplicationFactory Integration Test

**Why this is correct:**
The test uses WebApplicationFactory to boot the real application host and TestServer, then makes an actual HTTP call. It verifies that the endpoint is wired, DI resolves, routing works, and the pipeline returns the expected status code — without duplicating unit-level logic permutations.

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

**Key governance points:**
- Uses `IClassFixture<WebApplicationFactory<Program>>` for shared host setup.
- `factory.CreateClient()` routes requests through TestServer — no real network call.
- Test verifies HTTP-level behavior: routing, status code, serialization pipeline.
- Detailed business logic permutations belong in unit tests, not here.
- In production test projects, override `ConfigureWebHost` to replace real external dependencies with fakes.
