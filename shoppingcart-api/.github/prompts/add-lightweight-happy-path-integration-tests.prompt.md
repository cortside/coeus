---
description: "Add lightweight happy-path integration tests using WebApplicationFactory."
name: "Add Happy Path Integration Tests"
argument-hint: "Endpoint or pipeline behavior to verify"
---
Add deterministic integration tests for important API pipeline behavior.

Requirements:
- Use Microsoft.AspNetCore.Mvc.Testing and WebApplicationFactory.
- Focus on happy-path endpoint and dependency wiring behavior.
- Keep tests isolated from external services.
- Use test doubles/fakes/approved local alternatives.
- Keep broad permutation testing in unit tests.
