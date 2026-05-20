---
name: refactor-without-architecture-violations
description: Refactor safely while preserving architecture boundaries and behavior.
argument-hint: Refactor goal, target files, and observable behaviors that must remain unchanged
user-invocable: true
disable-model-invocation: false
---
# Refactor Without Architecture Violations Skill

## When To Use
Use when improving code clarity, reducing duplication, or restructuring internals where external behavior must remain unchanged.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Architecture Layering Governance](../../instructions/architecture-layering-governance.instructions.md)
- [Mapping Governance](../../instructions/mapping-governance.instructions.md)

## Step-By-Step Procedure
1. Define the refactor goal precisely: what is being cleaned up and why.
2. Identify existing tests that cover the behavior being refactored. Run them before starting to establish a green baseline.
3. Perform the refactor in the smallest increment that is independently testable.
4. After each increment: run tests to confirm no behavioral regression.
5. Check that each changed file remains within its correct layer:
   - Controller: only transport binding and delegation.
   - Facade: only orchestration, no HTTP/broker.
   - Domain service: only domain rules, no transport.
   - Repository: only persistence, no business logic.
6. Confirm explicit mapping is preserved — do not introduce AutoMapper as a refactor shortcut.
7. If a refactor naturally surfaces a boundary violation already present in the code, note it separately. Do not silently fix unrelated violations without user approval.
8. Run the full affected test suite before declaring the refactor complete.

## Acceptance Criteria
- All pre-existing tests pass after the refactor.
- No layer boundary violations introduced or silently fixed without approval.
- Mapping remains explicit.
- Scope is contained to the stated refactor goal.

## Edge Cases
- Extracting a method that crosses a layer boundary: flag this as an architecture decision, not a refactor.
- Renaming public contracts (DTOs, endpoints): these are behavior changes — stop and confirm with user.
- Removing dead code: confirm it is not called via reflection, DI registration, or integration test setup.

## Stop Conditions
- Stop after tests pass, scope is confirmed contained, and no new violations are introduced.
