# Hook: Git Safety Check

> **Platform enforcement**: This check is backed by `git-safety.json`, a real VS Code `PreToolUse` hook that intercepts terminal commands and blocks git write operations at the platform level. The checklist below documents the intent; the JSON hook provides the enforcement.

## Goal
Prevent prohibited git write/state-changing operations.

## Check
- Planned git commands are read-only.
- No add/commit/push/pull/merge/rebase/checkout/switch/reset/clean/stash operations.

## Action On Failure
Stop and request that the user performs required git write operations manually.
