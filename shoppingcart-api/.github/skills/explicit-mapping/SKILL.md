---
name: explicit-mapping
description: Implement explicit mapping functions and tests without AutoMapper.
argument-hint: Source model name, target model name, property rules, and layer boundary where mapping lives
user-invocable: true
disable-model-invocation: false
---
# Explicit Mapping Skill

## When To Use
Use when a new or changed mapping between DTOs, domain models, command models, or persistence entities is needed at any layer boundary.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Mapping Governance](../../instructions/mapping-governance.instructions.md)

## Step-By-Step Procedure
1. Determine the correct layer that owns the mapping:
   - Controller: request DTO → command/query model; facade result → response DTO.
   - Event handler: message contract → internal command/DTO.
   - Facade: domain entity/result → service DTO.
   - Repository: EF entity → domain entity; domain entity → EF entity.
2. Write a static or instance mapping method with explicit property-by-property assignments.
3. Use a clear naming convention consistent with the existing codebase (e.g. ToDto, ToDomain, ToEntity).
4. For nullable source fields: handle null explicitly — do not rely on implicit null propagation.
5. For enum conversions: cast or switch explicitly — do not rely on integer coercion.
6. For nested structures: call nested mapping functions recursively — do not inline deep property chains.
7. Write unit tests for:
   - All properties mapped correctly (happy path).
   - Default value behavior for optional/nullable fields.
   - Enum value round-trip correctness.
   - Null input handling.
   - Nested model mapping.
   - Computed or derived fields.

## Acceptance Criteria
- No AutoMapper or reflection-based mapping present.
- Mapping function is readable — reviewer can confirm correctness by inspection.
- Layer ownership is correct.
- Unit tests cover all non-trivial mapping cases.

## Edge Cases
- Collections: map each element using the same mapping function — do not use Select with anonymous lambdas for complex maps.
- Bidirectional mapping (domain ↔ entity): write separate functions for each direction.
- Record types: use constructor-based mapping for immutability.

## Stop Conditions
- Stop after mapping functions are written, correctly placed, and tests pass.
