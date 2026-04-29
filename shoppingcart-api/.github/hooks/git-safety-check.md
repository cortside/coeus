# Hook: Git Safety Check

## Goal
Prevent prohibited git write/state-changing operations.

## Check
- Planned git commands are read-only.
- No add/commit/push/pull/merge/rebase/checkout/switch/reset/clean/stash operations.

## Action On Failure
Stop and request that the user performs required git write operations manually.
