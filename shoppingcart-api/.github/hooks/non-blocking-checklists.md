# Non-Blocking Governance Checklists

Use these checks during review and completion, but do not run them as always-on blocking hooks.

## Mapping And AutoMapper Checklist
- AutoMapper is not introduced.
- No reflection-based or implicit generic mapping utilities.
- Mapping is explicit and at the correct layer boundary.

## Integration Test Expectation Checklist
- API and pipeline changes are identified.
- Lightweight integration tests exist for important infrastructure behavior.
- Tests use WebApplicationFactory and TestServer patterns.

## Troubleshooting Iteration Checklist
- Timestamped artifact folder exists.
- Current iteration note has inspected items, changes, rationale, tests, result, and next step.
- Explicit approval is captured before the next iteration.

## Plan Status Hygiene Checklist
- In Progress tasks are marked correctly.
- Done tasks include completion notes.
- Progress log has dated entries for major state changes.
