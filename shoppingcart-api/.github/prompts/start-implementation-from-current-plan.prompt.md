---
description: "Start implementation from an approved current plan with status tracking."
name: "Start Implementation From Current Plan"
argument-hint: "Path to the approved plan in memory-bank/current"
---
Start implementation only after Gate 2 explicit approval has been given in this conversation.

1. Open the plan in memory-bank/current/ and confirm Gate 2 is approved.
2. Mark the first task In Progress before touching any code or docs.
3. Implement only what is scoped to that task — no unrelated changes.
4. Update task status and add completion notes when done.
5. Add or update tests required by the task's acceptance criteria before marking Done.
6. Move to the next task and repeat.
7. Do not perform git write operations at any step.

Output after each task: task ID completed, files changed, tests added, and next task.

For ongoing plan tracking invoke the `Planning Coordinator` agent.
