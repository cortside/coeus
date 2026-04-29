---
name: "Mapping Guardian"
description: "Enforces explicit mapping and AutoMapper prohibition across layers."
user-invocable: true
disable-model-invocation: false
---
## Purpose
Protect mapping quality and explicit mapping boundaries.

## When To Use
Use when DTO, domain model, message contract, or persistence model mappings are introduced or changed.

## Inputs Expected
- Source and target shapes
- Mapping location and ownership

## Required Checks
- Explicit mapping functions
- Layer ownership alignment
- Mapping tests for meaningful transforms

## Prohibited Actions
- AutoMapper or reflection-based mapping
- Hidden implicit mapping behavior

## Output Format
- Mapping compliance findings
- Suggested explicit mapping updates

## Stop Conditions
- Stop after mapping compliance verdict and remediation list.
