# Governance Checklists

These documents define the policy checks Copilot is expected to apply at key workflow moments. They are **not** VS Code platform hooks — they cannot intercept operations automatically. They are instructions the model reads and follows voluntarily.

For executable platform hooks (shell commands that run at lifecycle events regardless of how the agent was prompted), see [hooks/README.md](../hooks/README.md).

## Pre-Implementation Gate

- [pre-implementation-planning-check.md](pre-implementation-planning-check.md) — confirms a plan exists and is approved before any implementation starts

## During Implementation

- [architecture-boundary-check.md](architecture-boundary-check.md) — verifies layer boundaries are respected before finalizing changes
- [test-completeness-check.md](test-completeness-check.md) — confirms tests are updated with behavioral changes before marking work done

## Completion Reminders (Non-Blocking)

- [non-blocking-checklists.md](non-blocking-checklists.md) — secondary quality and naming checks (AutoMapper, integration tests, troubleshooting iteration stops, plan status updates)

## Platform Hook Reference

- [git-safety-check.md](git-safety-check.md) — documents the intent of the `git-safety.json` platform hook that blocks git write operations at the PreToolUse event
