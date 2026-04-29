---
description: "Use when creating plans, writing phased implementation plans, converting requirements into tasks, tracking plan progress, or asking to begin implementation after planning."
name: "Planning Governance"
applyTo: "**"
---
# Planning Governance

## Purpose
Enforce a strict plan-first workflow before any implementation work begins.

## Required Workflow
1. Capture requirements in writing.
2. Create a requirements copy in `docs/`.
3. Create a phased plan in `memory-bank/planning/`.
4. Wait for explicit user approval of the plan.
5. Move approved plan to `memory-bank/current/`.
6. Wait again for explicit user approval to start implementation.
7. During implementation, update task completion status continuously.
8. After user verifies completion, mark the plan complete and move it to `memory-bank/completed/`.

## Hard Gates
- Never start implementation while drafting or revising a plan.
- Plan approval is not implementation approval.
- Implementation may start only after an explicit user message instructing implementation to begin.

## Requirements And Traceability Rules
- Requirements must be listed with stable IDs (`REQ-001`, `REQ-002`, ...).
- Every task must map to one or more requirement IDs.
- Every phase must include:
  - objective
  - in-scope tasks
  - out-of-scope items
  - dependencies and risks
  - exit criteria
- Include a traceability matrix that maps requirement IDs to task IDs.

## Completion Tracking Rules
- Tasks must use checkboxes and status fields (`Not Started`, `In Progress`, `Blocked`, `Done`).
- Keep a dated progress log in the active plan file.
- Update status immediately when work state changes.
- If blocked, record blocker, owner, and next action.

## File Naming Conventions
- Requirements copy: `docs/requirements-<topic>-<YYYYMMDD>.md`
- Draft plan: `memory-bank/planning/plan-<topic>-<YYYYMMDD>.md`
- Active approved plan: `memory-bank/current/plan-<topic>-<YYYYMMDD>.md`
- Completed plan: `memory-bank/completed/plan-<topic>-<YYYYMMDD>.md`

## Plan Output Minimum Sections
- Title and metadata (owner, date, status)
- Requirement catalog with IDs
- Assumptions and constraints
- Phased plan
- Task list with requirement traceability
- Traceability matrix
- Approval checkpoints
- Progress log
- Completion verification checklist
