---
name: add-unit-tests
description: Add unit tests with clear behavioral coverage and AAA structure.
argument-hint: Component name, method under test, and behaviors to cover
user-invocable: true
disable-model-invocation: false
---
# Add Unit Tests Skill

## When To Use
Use when behavior has been added or changed and test coverage must be established or verified.
Prefer the `add-unit-tests-for-layer` prompt for quick invocations.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Unit Testing Governance](../../instructions/unit-testing-governance.instructions.md)

## Step-By-Step Procedure
1. Identify all behaviors of the method or component: inputs, outputs, side effects, and exceptions.
2. Write one test per distinct behavior — do not combine multiple assertions for unrelated paths.
3. Follow Arrange / Act / Assert structure in every test.
4. Mock or stub external dependencies only where they are necessary to isolate the behavior under test.
5. Name tests using the pattern: `MethodName_ExpectedResult_WhenCondition`.
6. Cover the following paths at minimum:
   - Success path with valid inputs.
   - Validation failure path with invalid or missing inputs.
   - Error or exception path when a dependency fails.
   - Null or empty input path where applicable.
   - Important edge cases (boundary values, empty collections, enum transitions).
7. For mapping functions: write dedicated tests for default values, computed fields, enum conversions, null handling, and nested structure correctness.
8. Run tests and confirm all pass before marking work complete.

## Acceptance Criteria
- Every changed or added behavior has at least one passing test.
- Each test has a single, clear assertion focus.
- Tests do not break when implementation is refactored without behavior change.

## Edge Cases
- Components with many branches: cover each branch independently.
- Domain entities: test invariant enforcement and state transitions.
- Event handlers: test deserialization failures and delegation paths.

## Stop Conditions
- Stop after all identified paths have tests and tests pass.
