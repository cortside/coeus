# Non-Blocking Governance Checklists

Use these checks during review and completion, but do not run them as always-on blocking hooks.
For deep procedural guidance on any topic, see the relevant skill or prompt linked below.

## Mapping And AutoMapper Checklist
- AutoMapper is not introduced.
- No reflection-based or implicit generic mapping utilities.
- Mapping is explicit and at the correct layer boundary.

For detailed mapping guidance: use the `explicit-mapping` skill or `create-explicit-mapping-functions` prompt.

## Integration Test Expectation Checklist
- API and pipeline changes are identified.
- Lightweight integration tests exist for important infrastructure behavior.
- Tests use WebApplicationFactory and TestServer patterns.

For detailed integration test guidance: use the `add-integration-tests-webapplicationfactory` skill or `add-lightweight-happy-path-integration-tests` prompt.

## Troubleshooting Iteration Checklist
- Timestamped artifact folder exists.
- Current iteration note has inspected items, changes, rationale, tests, result, and next step.
- Explicit approval is captured before the next iteration.

For detailed troubleshooting guidance: use the `investigate-bug-troubleshooting-workflow` skill or `investigate-issue-without-changing-code` prompt.
For coordinator support: invoke `Troubleshooting Coordinator` agent.

## Plan Status Hygiene Checklist
- In Progress tasks are marked correctly.
- Done tasks include completion notes.
- Progress log has dated entries for major state changes.

For plan creation guidance: use the `planning-governance` skill or `plan-from-requirements` prompt.
For execution tracking: invoke `Planning Coordinator` agent.
