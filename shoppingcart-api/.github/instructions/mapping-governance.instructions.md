---
description: "Explicit mapping governance and AutoMapper prohibition."
name: "Mapping Governance"
applyTo: "src/**/*.cs"
---
# Mapping Governance

## Required Approach
- Mapping must be explicit, readable, and layer-owned.
- Controllers map request and response boundaries.
- Event handlers map message contracts to internal models.
- Facades map domain outputs to service DTOs.
- Repositories map persistence shapes to domain entities.

## Prohibited Tools
- AutoMapper is prohibited.
- Reflection-based magic mapping is prohibited.
- Implicit generic object mappers are prohibited.

## Test Expectations
- Add unit tests for meaningful mapping behavior including defaults, computed fields, enum conversion, nullability handling, and nested structures.
