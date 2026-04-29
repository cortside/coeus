---
name: planning-governance
description: 'Create governed phased plans from requirements with strict approval gates, task traceability, and lifecycle file moves across planning, current, and completed states.'
argument-hint: 'Provide requirements and topic for a phased plan'
user-invocable: true
disable-model-invocation: false
---

# Planning Governance Skill

## When To Use
- A user asks for a plan before implementation.
- Requirements need phased execution planning.
- Traceability between requirements and tasks is required.
- Progress tracking and approval gates must be enforced.

## Core Rules
- Never start implementation during plan creation.
- Plan approval is not implementation approval.
- Every task must map to at least one requirement ID.
- Keep status tracking current while work progresses.

## Workflow
1. Normalize incoming requirements into `REQ-###` IDs.
2. Create a requirements copy in `docs/` using the requirements template.
3. Create a draft plan in `memory-bank/planning/` using the plan template.
4. Add phases, tasks, dependencies, risks, and exit criteria.
5. Add a traceability matrix from requirements to tasks.
6. Add approval gates and stop for plan approval.
7. On plan approval, move plan to `memory-bank/current/` and wait for implementation-start approval.
8. On verified completion, mark complete and move to `memory-bank/completed/`.

## Templates
- Requirements template: [requirements-template.md](./assets/requirements-template.md)
- Plan template: [plan-template.md](./assets/plan-template.md)
- Workflow reference: [workflow.md](./references/workflow.md)
