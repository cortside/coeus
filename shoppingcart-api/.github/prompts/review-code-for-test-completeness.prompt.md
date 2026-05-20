---
description: "Review code changes for missing or insufficient test coverage."
name: "Review Test Completeness"
argument-hint: "Changed files or component names to assess for test coverage"
---
Review test coverage for the specified changes.

Check for:
- Unit tests covering success, failure, validation, and edge paths for each changed behavior.
- Integration tests for any new or changed API endpoints or pipeline behavior.
- Mapping tests for any significant model transformations.
- Tests that assert observable behavior rather than implementation details.

Output: findings by severity, specific missing test cases, and a pass/fail test-readiness verdict.

For specialist review invoke the `Test Engineer` agent.
