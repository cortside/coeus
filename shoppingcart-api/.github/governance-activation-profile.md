# Governance Activation Profile

This guide optimizes which governance artifacts should be used first so policy remains strong without unnecessary overhead.

## Mechanism Priority
1. Instructions: default source for standards and constraints.
2. Prompts and Skills: reusable workflows for common task execution.
3. Agents: specialist guidance for complex, focused work.
4. Hooks: small critical blocker set only.
5. Examples: reference patterns for implementation and review.

## Prompt → Skill → Agent Escalation Pattern

For any task, start with the most lightweight mechanism and escalate only when additional depth or specialization is needed:

| Start with | Use when |
|---|---|
| Prompt | You need a structured, repeatable workflow. Quick, focused invocation for common tasks. |
| Skill | You need a full step-by-step playbook with acceptance criteria, edge cases, and stop conditions. Use when the task is complex or the prompt is not enough. |
| Agent | You need specialist judgment, a dedicated role boundary, or a complex cross-file review. Use when a skill alone is not sufficient. |

**Example escalation for adding a repository method:**
1. Try: `add-efcore-repository-method-safely` prompt (quick invocation).
2. If more guidance is needed: use the `change-repository` skill (full procedure).
3. If specialist EF Core judgment is needed: invoke `EF Core Specialist` agent.

## Hook Budget
Keep the active platform hook focused on mechanical, pattern-matchable policy:
- `git-safety.json` (PreToolUse) — blocks git write operations

Governance checklists (planning pre-check, architecture boundary, test completeness, non-blocking) are in `.github/checklists/`. They are documentation the model reads voluntarily, not executable hooks.

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

Each agent has a `## When Not To Use` section in its agent file. Read it before invoking to confirm the agent scope matches your need.

Primary user-invocable agents:
- architecture-guardian.agent.md — architecture boundary review
- dotnet10-csharp-engineer.agent.md — .NET 10 / C# standards review
- ef-core-specialist.agent.md — EF Core query and persistence review
- test-engineer.agent.md — unit test coverage review
- planning-governor.agent.md — plan creation and traceability
- troubleshooting-coordinator.agent.md — approval-gated troubleshooting iterations

Support agents (not user-invocable):
- planning-coordinator.agent.md — post-approval tracking and lifecycle management

## Anti-Pattern Signals
If these occur, governance overhead is likely too high:
- Multiple hooks blocking the same concern in different wording.
- Agents with overlapping ownership for the same task stage.
- Prompts that duplicate instructions instead of operationalizing them.
- Frequent invocation of specialized prompts for common tasks.
- Skipping prompts and going directly to agents for routine changes.

## Quarterly Maintenance Checks
- Remove or merge artifacts not used in at least two real task runs.
- Keep instruction coverage complete while minimizing overlap.
- Keep hook set fixed unless a recurring incident justifies a new blocker.
- Verify each agent's When Not To Use section remains accurate relative to current prompt and skill coverage.
