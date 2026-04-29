---
description: "Start implementation from an approved current plan with status tracking."
name: "Start Implementation From Current Plan"
argument-hint: "Path to current approved plan"
---
Before implementation, verify Gate 2 explicit approval exists in the conversation.

Then:
1. Open the current plan in `memory-bank/current/`.
2. Set active tasks to In Progress before editing code/docs.
3. Implement only scoped tasks with requirement traceability.
4. Update status and completion notes as each task completes.
5. Add or update tests required by task acceptance criteria.
6. Do not perform git write operations.

Output:
- Tasks started and completed
- Files changed
- Validation and test evidence
- Remaining tasks
