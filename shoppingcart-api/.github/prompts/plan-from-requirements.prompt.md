---
description: "Create governed phased implementation plans from requirements with explicit approval gates and traceability."
name: "Plan From Requirements"
argument-hint: "Paste requirements or a path to requirements notes"
agent: "Planning Governor"
---
Create a governed, phased implementation plan from the provided requirements.

Follow this workflow exactly:
1. Normalize requirements into a numbered requirement catalog with IDs (`REQ-001`, `REQ-002`, ...).
2. Copy the normalized requirements to `docs/requirements-<topic>-<YYYYMMDD>.md`.
3. Create a phased plan at `memory-bank/planning/plan-<topic>-<YYYYMMDD>.md`.
4. Ensure every task has:
   - task ID (`TASK-###`)
   - description
   - owner (`Copilot` by default)
   - status (`Not Started` initially)
   - one or more linked requirement IDs
   - completion checkbox
5. Include a traceability matrix mapping each requirement to one or more tasks.
6. Include explicit gates:
   - `Gate 1: Plan Approval Required`
   - `Gate 2: Implementation Start Approval Required`
7. Stop after plan creation and ask for plan approval.

Do not start implementation. Plan approval does not authorize implementation.

If the user later approves the plan:
- Move the plan file from `memory-bank/planning/` to `memory-bank/current/`.
- Ask for explicit implementation-start approval.

If the user later verifies the implementation is complete:
- Mark all tasks done.
- Add completion verification notes.
- Move the plan file from `memory-bank/current/` to `memory-bank/completed/`.
