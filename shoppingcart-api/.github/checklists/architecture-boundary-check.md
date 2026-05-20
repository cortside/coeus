# Hook: Architecture Boundary Check

## Goal
Ensure layered architecture boundaries are preserved.

## Check
- Controllers and handlers delegate to facades.
- No business logic in controllers or handlers.
- No direct controller/handler repository or DbContext access.
- Repositories do not depend on upper layers.

## Action On Failure
Block completion and require boundary-compliant refactor.
