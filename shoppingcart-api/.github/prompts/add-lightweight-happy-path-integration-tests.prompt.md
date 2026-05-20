---
description: "Add lightweight happy-path integration tests using WebApplicationFactory."
name: "Add Happy Path Integration Tests"
argument-hint: "Endpoint route and expected HTTP status code for the happy path"
---
Add a lightweight integration test for an API endpoint pipeline.

1. Use WebApplicationFactory<Program> or the project's existing custom factory.
2. Configure test-specific services to replace external dependencies with fakes or in-memory alternatives.
3. Write one happy-path test per endpoint that verifies status code, routing, serialization, and DI wiring.
4. Keep detailed logic permutations in unit tests.
5. Ensure test is isolated and deterministic — no external service calls.

For a detailed step-by-step playbook use the `add-integration-tests-webapplicationfactory` skill.
