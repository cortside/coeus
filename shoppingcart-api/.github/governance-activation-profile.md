# Governance Activation Profile

This guide optimizes which governance artifacts should be used first so policy remains strong without unnecessary overhead.

## Mechanism Priority
1. Instructions: default source for standards and constraints.
2. Prompts and Skills: reusable workflows for common task execution.
3. Agents: specialist guidance for complex, focused work.
4. Hooks: small critical blocker set only.
5. Examples: reference patterns for implementation and review.

## Hook Budget
Keep blocking hooks to critical high-signal gates only:
- pre-implementation-planning-check.md
- git-safety-check.md
- architecture-boundary-check.md
- test-completeness-check.md

Secondary checks stay non-blocking in hooks/non-blocking-checklists.md.

## Prompt Activation Guidance
Primary prompts:
- plan-from-requirements.prompt.md
- add-endpoint-architecture-safe.prompt.md
- add-efcore-repository-method-safely.prompt.md
- add-unit-tests-for-layer.prompt.md
- add-lightweight-happy-path-integration-tests.prompt.md
- review-code-for-architecture-violations.prompt.md
- review-code-for-test-completeness.prompt.md

Process-specific prompts:
- review-plan-traceability.prompt.md
- start-implementation-from-current-plan.prompt.md
- investigate-issue-without-changing-code.prompt.md
- perform-one-approved-troubleshooting-iteration.prompt.md

Specialized prompts:
- create-explicit-mapping-functions.prompt.md

## Agent Activation Guidance
Primary user-invocable agents:
- architecture-guardian.agent.md
- aspnetcore-api-specialist.agent.md
- dotnet10-csharp-engineer.agent.md
- ef-core-specialist.agent.md
- mapping-guardian.agent.md
- test-engineer.agent.md
- integration-test-engineer.agent.md
- planning-governor.agent.md
- troubleshooting-coordinator.agent.md

Support agents:
- planning-coordinator.agent.md (post-approval tracking and lifecycle management)
- git-safety-guardian.agent.md (guardrail enforcement)

## Anti-Pattern Signals
If these occur, governance overhead is likely too high:
- Multiple hooks blocking the same concern in different wording.
- Agents with overlapping ownership for the same task stage.
- Prompts that duplicate instructions instead of operationalizing them.
- Frequent invocation of specialized prompts for common tasks.

## Quarterly Maintenance Checks
- Remove or merge artifacts not used in at least two real task runs.
- Keep instruction coverage complete while minimizing overlap.
- Keep hook set fixed unless a recurring incident justifies a new blocker.
