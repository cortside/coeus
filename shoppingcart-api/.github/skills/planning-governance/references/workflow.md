# Planning Governance Workflow

## Lifecycle
1. Requirements captured and normalized.
2. Requirements copy saved to `docs/requirements-<topic>-<YYYYMMDD>.md`.
3. Draft plan created at `memory-bank/planning/plan-<topic>-<YYYYMMDD>.md`.
4. Wait for Gate 1 approval.
5. Move approved plan to `memory-bank/current/plan-<topic>-<YYYYMMDD>.md`.
6. Wait for Gate 2 approval.
7. Execute implementation and update task statuses continuously.
8. User verifies completion.
9. Mark complete and move to `memory-bank/completed/plan-<topic>-<YYYYMMDD>.md`.

## Mandatory Governance Checks
- Every `TASK-###` maps to one or more `REQ-###` IDs.
- No orphan requirements in traceability matrix.
- No implementation before Gate 2 approval.
- Plan approvals and transitions recorded in progress log.

## Suggested Progress Log Entries
- `Plan drafted and awaiting Gate 1 approval.`
- `Gate 1 approved; moved to memory-bank/current. Awaiting Gate 2 approval.`
- `Gate 2 approved; implementation started.`
- `Implementation complete; awaiting user verification.`
- `User verified complete; plan moved to memory-bank/completed.`
