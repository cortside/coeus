---
description: "Integration testing governance based on ASP.NET Core TestServer and WebApplicationFactory."
name: "Integration Testing Governance"
applyTo: "src/**/*.IntegrationTests/**/*.cs"
---
# Integration Testing Governance

## Framework
- Use `Microsoft.AspNetCore.Mvc.Testing` and `WebApplicationFactory<TEntryPoint>`.
- Use test server and test client for SUT hosting.

## Scope
- Cover important endpoint and pipeline wiring behavior.
- Prefer happy-path infrastructure checks.
- Keep detailed business permutations in unit tests.

## Reliability
- Keep tests deterministic and isolated.
- Avoid dependence on external services.
- Use test doubles, fakes, containers, or approved local alternatives.
- Seed test data intentionally and clean up when required.
