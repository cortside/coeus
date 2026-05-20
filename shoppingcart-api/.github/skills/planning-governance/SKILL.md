---
name: planning-governance
description: 'Create governed phased plans from requirements with strict approval gates, task traceability, and lifecycle file moves across planning, current, and completed states.'
argument-hint: 'Provide requirements and topic for a phased plan'
user-invocable: true
disable-model-invocation: false
context: fork
---

# Planning Governance Skill

## When To Use
- A user asks for a plan before implementation.
- Requirements need phased execution planning.
- Traceability between requirements and tasks is required.
- Progress tracking and approval gates must be enforced.

## When Not To Use
Do not use during active implementation after Gate 2 approval. At that point use the `start-implementation-from-current-plan` prompt or `Planning Coordinator` agent for execution tracking.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Planning Governance](../../instructions/planning-governance.instructions.md)

## Core Rules
- Never start implementation during plan creation.
- Plan approval is not implementation approval.
- Every task must map to at least one requirement ID.
- Keep status tracking current while work progresses.

## Step-By-Step Procedure
1. Normalize incoming requirements into `REQ-###` IDs.
2. Create a requirements copy in `docs/` using the requirements template and naming convention `docs/requirements-<topic>-<YYYYMMDD>.md`.
3. Create a draft plan in `memory-bank/planning/` using the plan template and naming convention `memory-bank/planning/plan-<topic>-<YYYYMMDD>.md`.
4. Add phases with: objective, in-scope tasks, out-of-scope items, dependencies, risks, and exit criteria.
5. Add tasks with: task ID, owner, status (Not Started), linked REQ-### IDs, acceptance criteria, and test expectations.
6. Add a traceability matrix from REQ-### to TASK-###.
7. Add Gate 1 and Gate 2 approval checkpoint sections. Stop for Gate 1 approval after plan creation.
8. On Gate 1 approval, move plan to `memory-bank/current/` and stop for Gate 2 implementation-start approval.
9. On verified completion, mark all tasks Done and move plan to `memory-bank/completed/`.

## Acceptance Criteria
- Every requirement has a stable `REQ-###` ID.
- Every task has a stable `TASK-###` ID linked to at least one requirement.
- Gate 1 and Gate 2 checkpoints are explicit sections in the plan.
- File lifecycle moves match the approval sequence: planning → current → completed.
- Traceability matrix covers all requirements.

## Edge Cases
- Requirements conflict: document the conflict as an assumption or constraint and ask for clarification before continuing.
- Large requirement sets: split into phases with explicit out-of-scope declarations per phase.
- Mid-implementation requirement changes: create a new requirements addendum and update the plan before continuing.

## Stop Conditions
- Stop after plan creation and request Gate 1 approval.
- After Gate 1 move, stop and request Gate 2 implementation-start approval.
- After Gate 2, implementation begins — do not gate individual tasks.

## Templates
- Requirements template: [requirements-template.md](./assets/requirements-template.md)
- Plan template: [plan-template.md](./assets/plan-template.md)
- Workflow reference: [workflow.md](./references/workflow.md)
