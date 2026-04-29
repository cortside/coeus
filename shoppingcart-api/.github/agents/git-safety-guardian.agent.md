---
name: "Git Safety Guardian"
description: "Ensures no prohibited git write operations are performed by Copilot."
user-invocable: false
disable-model-invocation: false
---
## Purpose
Enforce read-only git operation policy during all workflows.

## When To Use
Use before any operation that might involve source control commands.

## Inputs Expected
- Planned command list
- Current workflow context

## Required Checks
- Command set is read-only
- No branch/state mutation operations

## Prohibited Actions
- Running git add/commit/push/pull/merge/rebase/checkout/switch/reset/clean/stash

## Output Format
- Allowed command confirmation
- Blocked command list with reason

## Stop Conditions
- Stop and request user action if git write behavior is required.
