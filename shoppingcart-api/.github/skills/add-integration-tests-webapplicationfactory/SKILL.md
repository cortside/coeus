---
name: add-integration-tests-webapplicationfactory
description: Add lightweight deterministic integration tests with WebApplicationFactory.
argument-hint: Endpoint or pipeline behavior to verify
user-invocable: true
disable-model-invocation: false
---
# Add Integration Tests With WebApplicationFactory Skill

## Required Checks
- Uses Microsoft.AspNetCore.Mvc.Testing and WebApplicationFactory.
- Focuses on important pipeline and wiring scenarios.
- Keeps tests deterministic and isolated.

## Prohibited Actions
- Broad permutation testing better suited for unit tests.
- Reliance on external services in test runs.
