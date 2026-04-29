---
description: "Unit testing governance for all service layers."
name: "Unit Testing Governance"
applyTo: "src/**/*.Tests/**/*.cs"
---
# Unit Testing Governance

## Expectations
- Unit tests are mandatory for behavioral changes.
- Cover controller, handler, facade, domain service, repository (where practical), entity, mapper, and validator behavior.
- Use clear Arrange, Act, Assert structure.

## Coverage Focus
- Success and failure paths.
- Validation and edge, null, empty handling.
- Authorization branches where applicable.
- Mapping correctness for significant transformations.

## Quality Rules
- Prefer fast, isolated tests.
- Avoid brittle tests coupled to internal implementation details.
