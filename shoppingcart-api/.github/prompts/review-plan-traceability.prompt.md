---
description: "Review an implementation plan for requirement and task traceability completeness."
name: "Review Plan Traceability"
argument-hint: "Path to plan file and requirements file"
---
Review the provided plan and requirements files for traceability quality.

Checks:
1. Every requirement ID maps to one or more task IDs.
2. Every task maps to one or more requirement IDs.
3. Every task has owner, status, acceptance criteria, and test expectations.
4. Plan includes Gate 1 and Gate 2 checkpoints.
5. Plan includes progress log and completion checklist.

Output:
- Findings by severity
- Missing links or orphaned requirements/tasks
- Suggested corrections
- Pass or fail verdict
