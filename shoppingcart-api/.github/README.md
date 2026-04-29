# Copilot Governance Index

This repository uses a governance-first Copilot model aligned to Coeus layered architecture and strict safety rules.

## Quick Start
- Read `copilot-instructions.md` for repository-wide defaults.
- Read instruction files in `instructions/` for mandatory standards.
- Use prompts in `prompts/` for repeatable workflows.
- Use skills in `skills/` for domain-specific procedures.
- Use agents in `agents/` for role-specialized execution.
- Use a small set of hook policies in `hooks/` for critical blocking checks only.
- Use `governance-activation-profile.md` to choose the right governance artifact for the task.
- Review `examples/` for correct and incorrect patterns.

## Governance Operating Model
- Instructions are the primary policy mechanism and should carry most standards.
- Prompts and skills should encode reusable workflows, not global policy.
- Agents should be specialized for narrow roles and avoid overlap.
- Hooks should be used judiciously: only high-value blockers should remain always-on.
- Secondary checks should be maintained in non-blocking checklists.

## Governance Categories
- Architecture and layering
- .NET 10 and C# coding standards
- ASP.NET Core API standards
- EF Core standards
- Explicit mapping standards
- Unit and integration testing standards
- Git safety constraints
- Planning lifecycle controls
- Troubleshooting workflow controls
- Documentation standards

## Safety Model
- Git write operations are prohibited for Copilot.
- Plan approval is not implementation approval.
- Troubleshooting runs one approved iteration at a time.
