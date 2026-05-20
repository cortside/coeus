---
description: "Review an implementation plan for requirement and task traceability completeness."
name: "Review Plan Traceability"
argument-hint: "Paths to the plan file and requirements file in memory-bank/current and docs"
---
Review the plan and requirements files for traceability quality.

1. Verify every REQ-### maps to at least one TASK-###.
2. Verify every TASK-### maps back to at least one REQ-###.
3. Confirm each task has: owner, status field, acceptance criteria, and test expectations.
4. Confirm Gate 1 and Gate 2 checkpoints are present.
5. Confirm a progress log and completion checklist exist.

Output findings by severity, list orphaned requirements or tasks, suggest corrections, and give a pass/fail verdict.

For formal plan governance invoke the `Planning Governor` agent.
