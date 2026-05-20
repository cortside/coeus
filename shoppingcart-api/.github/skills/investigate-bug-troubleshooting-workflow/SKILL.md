---
name: investigate-bug-troubleshooting-workflow
description: Investigate issues using artifacted, approval-gated troubleshooting iterations.
argument-hint: Issue description, observed vs expected behavior, and any known reproduction steps
user-invocable: true
disable-model-invocation: false
context: fork
---
# Investigate Bug Troubleshooting Workflow Skill

## When To Use
Use whenever a user asks to fix, debug, investigate, or troubleshoot an issue. Never skip this workflow and jump directly to code changes.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Troubleshooting Workflow Governance](../../instructions/troubleshooting-workflow-governance.instructions.md)

## Step-By-Step Procedure

### Iteration 1 — Analysis Only
1. Create the artifact folder: `artifacts/YYYYMMDD-HHMMSS-short-description/`
2. Create `iteration-001-analysis.md` in that folder.
3. In the file document:
   - Observed behavior.
   - Expected behavior.
   - Files, components, and layers inspected.
   - Root cause candidates ranked by likelihood.
   - Recommended next step.
4. Do not change any implementation code in this iteration.
5. Stop and request user approval to proceed.

### Iteration 2+ — Change And Test (One Per Approval)
1. Create the next iteration file: `iteration-002-change-and-test.md`, `iteration-003-follow-up.md`, etc.
2. Implement only the change approved in the previous iteration note.
3. Run relevant unit and integration tests.
4. Document in the iteration file:
   - What was inspected.
   - What was changed.
   - Why it was changed.
   - Tests run and results.
   - Whether the issue is resolved.
   - Next recommended step if not resolved.
5. Stop after this single iteration and request approval before continuing.

## Acceptance Criteria
- Timestamped artifact folder exists before any code change.
- Each iteration has its own notes file.
- No implementation change happens without a preceding approved analysis.
- The issue root cause is documented even when resolved.

## Edge Cases
- Issue cannot be reproduced: document reproduction attempts in iteration-001 and ask user for additional context before proceeding.
- Multiple plausible root causes: address one per iteration; do not batch fixes.
- Fix introduces a regression: create a new iteration documenting the regression and revert recommendation.

## Stop Conditions
- Stop after each iteration and wait for approval. Only proceed when the user explicitly authorizes the next step.
