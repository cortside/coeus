---
description: "Review code changes for architecture and boundary violations."
name: "Review Architecture Violations"
argument-hint: "File paths or diff to review"
---
Review the specified files or diff for architecture boundary violations.

Check each tier:
- Controller/handler: no business logic, no direct repository or DbContext calls.
- Facade: no HTTP or broker concerns, delegates to domain services.
- Domain service: no HTTP concerns, no controller DTOs, uses repository abstractions.
- Repository: no business logic, no upward-layer dependencies.
- Domain entity: no DI, no infrastructure concerns.

Output findings with severity (blocking/warning), exact file and line references, and required remediation steps.

For specialist review invoke the `Architecture Guardian` agent.
