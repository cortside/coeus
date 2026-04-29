---
name: add-api-endpoint
description: Add a new API endpoint while preserving controller-facade-domain-repository boundaries.
argument-hint: Endpoint route, request shape, response shape, and behavior
user-invocable: true
disable-model-invocation: false
---
# Add API Endpoint Skill

## Required Checks
- Controller remains thin.
- Facade orchestrates business workflow.
- No direct controller repository or DbContext access.
- Explicit mapping only.
- Required unit and integration tests are added or updated.

## Prohibited Actions
- AutoMapper introduction.
- Git write operations.
