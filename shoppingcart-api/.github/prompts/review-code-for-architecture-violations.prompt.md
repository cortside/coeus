---
description: "Review code changes for architecture and boundary violations."
name: "Review Architecture Violations"
argument-hint: "Files or diff to review"
---
Review code for architecture violations.

Check for:
- Controller or handler business logic
- Controller direct repository or DbContext access
- Facade transport concerns
- Domain service transport concerns
- Repository upward-layer dependencies
- Domain entity infrastructure leakage

Output findings with severity and exact file references.
