---
name: add-api-endpoint
description: Add a new API endpoint while preserving controller-facade-domain-repository boundaries.
argument-hint: Endpoint route, HTTP method, request shape, response shape, and facade target
user-invocable: true
disable-model-invocation: false
---
# Add API Endpoint Skill

## When To Use
Use when adding or changing a controller action and its downstream layers across the full stack.
Prefer the `add-endpoint-architecture-safe` prompt for quick, focused invocations.

## Governing Rules
This skill enforces the rules in the following instruction files. Read them before proceeding:
- [Architecture Layering Governance](../../instructions/architecture-layering-governance.instructions.md)
- [ASP.NET Core API Governance](../../instructions/aspnetcore-api-governance.instructions.md)
- [Mapping Governance](../../instructions/mapping-governance.instructions.md)

## Step-By-Step Procedure
1. Define request DTO and response DTO in the appropriate Dto project.
2. Add the controller action: apply route attribute, HTTP method attribute, and `[ProducesResponseType]` annotations consistent with existing patterns.
3. Perform model validation in the action. Return `ValidationProblem(ModelState)` on invalid input.
4. Map the request DTO to an internal command or query model inside the controller action.
5. Call the facade method, passing the command and CancellationToken.
6. Map the facade result to a response DTO inside the controller action.
7. Return appropriate status code: 201 + `CreatedAtAction` for creates, 200 for reads, 204 for deletes with no body.
8. Add or update the facade method to orchestrate domain services.
9. Add or update domain service and repository methods as needed per their respective governance.
10. Add unit tests: controller happy path, validation failure, facade error propagation.
11. Add lightweight integration test: HTTP response status and body shape for the happy path.

## Acceptance Criteria
- Controller action contains no business logic.
- No direct repository or DbContext reference in the controller.
- All mapping is explicit — no AutoMapper.
- Unit tests cover success, validation failure, and at least one error path.
- Integration test verifies the endpoint wires and returns expected status code.

## Edge Cases
- Nullable route parameters: validate and return 404 if not found.
- Conflicting routes: align with existing versioning and route conventions.
- Facade exceptions: map to appropriate HTTP problem detail responses consistent with existing error handling.

## Stop Conditions
- Stop after tests pass and boundary compliance is confirmed.
