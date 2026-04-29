# Instructions Index

Use these instruction files as mandatory repository governance rules.

## Core
- global-repository-governance.instructions.md
- git-safety-governance.instructions.md
- documentation-standards.instructions.md

## Architecture and Runtime
- architecture-layering-governance.instructions.md
- aspnetcore-api-governance.instructions.md
- dotnet10-csharp-standards.instructions.md
- efcore-governance.instructions.md
- mapping-governance.instructions.md

## Quality and Process
- unit-testing-governance.instructions.md
- integration-testing-governance.instructions.md
- planning-governance.instructions.md
- troubleshooting-workflow-governance.instructions.md

## Scope Tuning Notes
- Keep `applyTo` broad only for truly global rules.
- Prefer project-targeted `applyTo` patterns for EF Core and API governance to reduce activation noise.
- Revisit `applyTo` patterns when new top-level projects are added under `src/`.
