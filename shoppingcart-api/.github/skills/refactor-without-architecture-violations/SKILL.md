---
name: refactor-without-architecture-violations
description: Refactor safely while preserving architecture boundaries and behavior.
argument-hint: Refactor goal and target files
user-invocable: true
disable-model-invocation: false
---
# Refactor Without Architecture Violations Skill

## Required Checks
- Preserve controller-facade-domain-repository boundaries.
- Keep behavior unchanged unless explicitly requested.
- Maintain explicit mapping rules and test coverage.

## Prohibited Actions
- Layer boundary violations.
- Unrelated broad refactors.
