---
name: explicit-mapping
description: Implement explicit mapping functions and tests without AutoMapper.
argument-hint: Source and target model types plus mapping rules
user-invocable: true
disable-model-invocation: false
---
# Explicit Mapping Skill

## Required Checks
- Mapping remains explicit and readable.
- Mapping location matches layer boundary ownership.
- Tests validate defaults, enums, nested shapes, and nullability.

## Prohibited Actions
- AutoMapper.
- Reflection-based or implicit generic mapping.
