# Governance Hooks

## Platform Hooks vs. Governance Checklists

VS Code loads hook behavior **only from `.json` files** in `.github/hooks/`. These execute shell commands at key lifecycle points (PreToolUse, PostToolUse, SessionStart, Stop, etc.) with guaranteed outcomes regardless of how the agent was prompted.

The `.md` files in this directory are **governance checklists** — documents the model reads and follows. They are not loaded as platform hooks and cannot intercept operations.

## Active Platform Hooks (Executable)

| File | Event | Enforces |
|------|-------|----------|
| `git-safety.json` | `PreToolUse` | Blocks all git write operations at the platform level |

Scripts are in `scripts/`. Requires execute permission on macOS/Linux: `chmod +x .github/hooks/scripts/*.sh`.

## Governance Checklists

The governance checklists (planning pre-check, architecture boundary check, test completeness, non-blocking reminders, and git-safety intent doc) have moved to [`.github/checklists/`](../checklists/README.md).

## Hook Budget
- Keep platform hooks focused on mechanical, pattern-matchable policy (git commands, secret patterns).
- Do not try to enforce semantic rules ("is this the right layer?") via shell hooks; those belong in instruction files and skills.
