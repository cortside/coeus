---
name: change-event-handler
description: Add or modify message event handlers with strict transport-layer boundaries.
argument-hint: Message contract and handling behavior
user-invocable: true
disable-model-invocation: false
---
# Change Event Handler Skill

## Required Checks
- Handler receives and validates message contracts.
- Handler maps message shape to internal models explicitly.
- Handler delegates workflow to facades.
- Unit tests cover deserialization, validation, and delegation behavior.

## Prohibited Actions
- Business logic in handlers.
- Direct repository or DbContext access.
