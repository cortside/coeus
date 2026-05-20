# Correct: Plan Format

**Why this is correct:**
The plan has stable IDs for requirements and tasks, explicit gate checkpoints, status tracking per task, and links every task to at least one requirement. Implementation cannot start until both gates are explicitly approved.

```markdown
# Plan: Topic
- Owner: Copilot
- Status: Draft

## Approval Checkpoints
- Gate 1: Plan Approval Required
- Gate 2: Implementation Start Approval Required

## Requirements
- REQ-001: ...

## Tasks
- [ ] TASK-001
  - Owner: Copilot
  - Status: Not Started
  - Linked Requirements: REQ-001
  - Acceptance Criteria: ...
  - Test Expectations: ...
  - Completion Notes: Pending
```

**Key governance points:**
- Gate 1 and Gate 2 are distinct approvals — plan approval is not implementation approval.
- Every task maps to at least one REQ-### ID.
- Status field is maintained as work progresses.
- Completion Notes populated only when task is Done with evidence.
