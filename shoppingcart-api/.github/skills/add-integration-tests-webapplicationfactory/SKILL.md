---
name: add-integration-tests-webapplicationfactory
description: Add lightweight deterministic integration tests with WebApplicationFactory.
argument-hint: Endpoint route, HTTP method, and expected happy-path status code
user-invocable: true
disable-model-invocation: false
---
# Add Integration Tests With WebApplicationFactory Skill

## When To Use
Use when a new endpoint or significant pipeline behavior (middleware, auth, model binding, DI wiring) needs infrastructure-level verification.
Prefer the `add-lightweight-happy-path-integration-tests` prompt for quick invocations.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Integration Testing Governance](../../instructions/integration-testing-governance.instructions.md)

## Step-By-Step Procedure
1. Confirm the integration test project exists (typically `Acme.ShoppingCart.WebApi.IntegrationTests`). If absent, note this as a prerequisite.
2. Use `WebApplicationFactory<Program>` or the project's existing custom factory base class.
3. Override `ConfigureWebHost` to replace external dependencies: use in-memory databases, fakes, or approved test doubles.
4. Create an `HttpClient` via `factory.CreateClient()`.
5. Write one happy-path test per endpoint that:
   - Sends a valid request.
   - Asserts the expected HTTP status code.
   - Optionally asserts response body shape for key fields.
6. Write one test for each important infrastructure boundary: auth rejection (401/403), routing miss (404), model validation rejection (400).
7. Seed any required test data before the request and clean it up if the store is persistent.
8. Keep all logic permutation tests in unit tests — do not duplicate them here.

## Acceptance Criteria
- Each test uses WebApplicationFactory and makes real HTTP calls through TestServer.
- No test calls real external services (database, message broker, REST API).
- Tests are deterministic — same result every run regardless of order.
- Happy-path test exists for every new public endpoint.

## Edge Cases
- Endpoints with authentication: provide a valid test token or configure test auth handler.
- Endpoints that produce events: replace the message broker with a fake that captures published messages.
- Shared factory state: use `IClassFixture` for expensive setup, `CreateClient` per test for isolation.

## Stop Conditions
- Stop after happy-path and infrastructure-boundary tests pass and external isolation is confirmed.
