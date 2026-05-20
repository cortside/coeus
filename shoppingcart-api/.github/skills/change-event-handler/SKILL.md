---
name: change-event-handler
description: Add or modify message event handlers with strict transport-layer boundaries.
argument-hint: Message contract shape, broker type (RabbitMQ etc.), and facade method to delegate to
user-invocable: true
disable-model-invocation: false
---
# Change Event Handler Skill

## When To Use
Use when adding or modifying a handler that processes messages from a message broker (e.g. RabbitMQ) and delegates to a facade.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Architecture Layering Governance](../../instructions/architecture-layering-governance.instructions.md)
- [Mapping Governance](../../instructions/mapping-governance.instructions.md)

## Step-By-Step Procedure
1. Define or confirm the message contract type. Keep broker-specific types isolated to the handler.
2. Validate the incoming message: null check, required field presence, and any format constraints. Throw or return dead-letter signal on invalid messages consistent with existing patterns.
3. Map the message contract to an internal command or DTO explicitly — no AutoMapper.
4. Call the appropriate facade method with the command and CancellationToken.
5. Handle facade exceptions: log with structured logging (no sensitive data), and re-throw or dead-letter consistently with existing handler patterns.
6. Do not implement business logic in the handler.
7. Do not access repositories or DbContext directly.
8. Add unit tests covering:
   - Happy-path: valid message → correct facade call.
   - Null message: ArgumentNullException or equivalent guard.
   - Invalid message fields: validation rejection behavior.
   - Facade exception: correct error handling path.

## Acceptance Criteria
- Handler validates the message before delegation.
- Mapping from message to internal model is explicit.
- Facade is the only business workflow entry point — no domain service or repository calls directly.
- Unit tests cover all four paths above.

## Edge Cases
- Idempotency: if the broker may redeliver, confirm the facade or domain service handles duplicate detection.
- Poison messages: confirm dead-lettering or retry behavior aligns with the broker's configured policy.
- Schema evolution: if message contracts change, ensure backward compatibility or explicit version handling.

## Stop Conditions
- Stop after handler boundary compliance is confirmed and tests pass.
