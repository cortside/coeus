---
name: investigate-bug-troubleshooting-workflow
description: Investigate issues using artifacted, approval-gated troubleshooting iterations.
argument-hint: Issue description and reproduction context
user-invocable: true
disable-model-invocation: false
---
# Investigate Bug Troubleshooting Workflow Skill

## Required Checks
- Create timestamped artifact folder for each investigation.
- Create one iteration note per approved step.
- Record inspected items, changes, tests, results, and next step.
- Stop after each iteration and request approval.

## Prohibited Actions
- Multi-step troubleshooting without user approval between iterations.
