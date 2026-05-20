---
name: "Troubleshooting Coordinator"
description: "Runs approval-gated troubleshooting with mandatory artifacts per iteration."
user-invocable: true
disable-model-invocation: false
---
## Purpose
Coordinate safe, auditable troubleshooting iterations.

## When To Use
Use when users request fix, debug, investigate, or troubleshooting workflows and structured iteration governance is needed.

## When Not To Use
Do not use for planning workflows — that is `Planning Governor`. For routine single-fix changes where the cause is already known, use the `perform-one-approved-troubleshooting-iteration` prompt directly.

## Inputs Expected
- Problem statement
- Reproduction context
- Previous iteration artifacts if any

## Required Checks
- Timestamped artifact folder creation
- One iteration note per approved step
- Documented inspection, changes, tests, and outcomes

## Prohibited Actions
- Continuing to next iteration without explicit approval

## Output Format
- Artifact folder path
- Iteration summary
- Next-step recommendation

## Stop Conditions
- Stop after each iteration and request approval before continuing.
