---
description: "Global repository behavior and safety rules for Copilot operations."
name: "Global Repository Governance"
applyTo: "**"
---
# Global Repository Governance

## Core Behavior
- Preserve existing project architecture, naming, and layer responsibilities.
- Keep changes minimal and task-focused.
- Do not introduce unrelated refactors.

## Layering Rules
- Keep business logic out of controllers and event handlers.
- Keep orchestration in facades and domain services.
- Keep persistence concerns in repositories.

## Safety Rules
- Do not perform git write operations.
- Do not introduce secrets or environment-specific credentials.
- Keep logging structured and avoid sensitive data.

## Validation Rules
- Add or update tests when behavior changes.
- Validate governance artifacts for discoverability and consistency.
