# Copilot Governance Guide

This document explains every governance artifact in `.github/`, why it was created, what it does, and how to use it.

New to Copilot or generative AI? Start here first:

- [Copilot Governance Beginner Quickstart](CopilotGovernanceBeginnerQuickstart.md)
- [Copilot Governance One Page](CopilotGovernanceOnePage.md)

---

## Table of Contents

1. [Overview and Entry Point](#1-overview-and-entry-point)
2. [Instructions](#2-instructions)
3. [Prompts](#3-prompts)
4. [Skills](#4-skills)
5. [Agents](#5-agents)
6. [Hooks](#6-hooks)
7. [Examples](#7-examples)
8. [Governance Activation Profile](#8-governance-activation-profile)

---

## 1. Overview and Entry Point

### `.github/copilot-instructions.md`

**Why it was created:** This is the repository-wide default context file that Copilot loads automatically for every conversation. It needed to carry the non-negotiable constraints that apply regardless of what the user is working on — architecture flow, mapping rules, testing expectations, git safety, and the planning/troubleshooting gates.

**What it does:** Provides a compact summary of all governance areas so Copilot always has baseline awareness without requiring any user action.

**How to use it:** You do not invoke it directly. It activates automatically. Think of it as the always-on backstop. All other artifacts layer on top of it for specific tasks.

---

## 2. Instructions

Instructions are `.instructions.md` files in `.github/instructions/`. Copilot loads them automatically based on the `applyTo` glob pattern when you are working in matching files. They encode standing rules, not workflows.

> **Required VS Code setting:** `chat.includeApplyingInstructions` must be `true` for `applyTo` pattern-based instructions to apply automatically. Verify this is enabled in your VS Code settings.

### `global-repository-governance.instructions.md`

**Why:** Every Copilot session in this repo needs awareness of architecture preservation, layering, minimal change scope, and safety rules — regardless of what file is open.

**What it does:** Sets universal behavior: preserve architecture, keep changes task-focused, avoid git writes, keep logging clean, and require tests with behavioral changes. Also enforces Responsible AI rules: no secrets or payment data in prompts, mandatory human review for auth/payment/secrets changes, and prompt injection awareness.

**applyTo:** `**` (all files)

**Example trigger:** You open any file and ask Copilot for help. These rules are always active.

---

### `git-safety-governance.instructions.md`

**Why:** Copilot must never perform destructive git operations like `git reset`, `git push`, or `git stash`. The only safe git commands are read-only inspection commands.

**What it does:** Explicitly lists allowed read-only commands and prohibits all write and state-changing operations. If a task requires a write operation, Copilot must stop and ask the user to perform it.

**applyTo:** `**` (all files)

**Example trigger:**
> "Commit these changes for me."

Copilot will stop and ask you to run the commit yourself instead of executing `git add` and `git commit`.

---

### `architecture-layering-governance.instructions.md`

**Why:** The Coeus architecture has a strict flow: Controller → Facade → Domain Service → Repository. Violations — like a controller calling a repository directly, or a repository calling a facade — are hard to spot in code review and expensive to fix later.

**What it does:** Defines which tier owns which responsibility, what each tier is allowed to call, and what each tier is prohibited from doing.

**applyTo:** `src/**/*.cs`

**Example trigger:** You open `OrderController.cs` and ask Copilot to add order retrieval logic. Copilot will delegate to a facade rather than calling the repository directly.

---

### `aspnetcore-api-governance.instructions.md`

**Why:** Controllers in both `WebApi` and `CatalogApi` projects are frequently touched and must stay thin. Without explicit rules, Copilot might add business logic inside a controller action.

**What it does:** Defines controller responsibilities (bind, validate, delegate, map) and prohibitions (no business logic, no EF access, no direct repository calls). Also enforces correct HTTP status codes, model validation, and route/versioning preservation.

**applyTo:** `src/Acme.ShoppingCart.*Api/**/*.cs`

**Example trigger:** You ask Copilot to add a new POST endpoint. Copilot will create a thin action method that calls a facade method rather than embedding logic inline.

---

### `dotnet10-csharp-standards.instructions.md`

**Why:** The project targets .NET 10 and C# modern patterns. Common anti-patterns like `.Result`, sync-over-async, service locator, and swallowed exceptions needed to be explicitly prohibited.

**What it does:** Enforces nullable reference types, async/await with `CancellationToken`, constructor injection, structured logging, and no secrets in logs.

**applyTo:** `src/**/*.cs`

**Example trigger:** You ask Copilot to add a method that calls an external service. Copilot will write it as `async Task<T>` with a `CancellationToken` parameter rather than a synchronous wrapper.

---

### `efcore-governance.instructions.md`

**Why:** EF Core has well-known performance footguns: N+1 queries, accidental client-side evaluation, missing `AsNoTracking()` on read queries, and `IQueryable` leaking outside repository boundaries. These needed explicit rules scoped to where EF is actually used.

**What it does:** Enforces `AsNoTracking()` for read-only queries, async EF APIs, intentional use of includes and projections, no `IQueryable` exposure beyond allowed boundaries, and no migrations without explicit request.

**applyTo:** `src/**/*.Data/**/*.cs` (narrowed to data layer only where EF is used)

**Example trigger:** You ask Copilot to add a query method in a repository. Copilot will include `AsNoTracking()` and `CancellationToken` automatically.

---

### `mapping-governance.instructions.md`

**Why:** AutoMapper and reflection-based mappers were prohibited in this project. Without an explicit rule, Copilot might suggest `CreateMap<>` or `.Map<>()` calls. Mapping violations can appear in any layer.

**What it does:** Requires explicit, hand-written mapping at each layer boundary. Prohibits AutoMapper, reflection-based mappers, and implicit generic mappers. Also requires unit tests for meaningful mapping behavior.

**applyTo:** `src/**/*.cs` (broad scope because violations can appear in any layer)

**Example trigger:** You ask Copilot to map a domain entity to a DTO. Copilot will write an explicit `new OrderDto { ... }` constructor call rather than suggesting AutoMapper.

---

### `unit-testing-governance.instructions.md`

**Why:** Tests were required for every behavioral change but the standards for structure, coverage scope, and test quality needed to be explicit.

**What it does:** Requires AAA structure, coverage of success/failure paths, validation edge cases, null/empty handling, and authorization branches. Prohibits brittle tests tightly coupled to implementation details.

**applyTo:** `src/**/*.Tests/**/*.cs`

**Example trigger:** You ask Copilot to write tests for a facade method. Copilot will produce tests covering the happy path, validation failure, and null inputs — not just a single pass-through test.

---

### `integration-testing-governance.instructions.md`

**Why:** Integration tests in this project use `WebApplicationFactory<TEntryPoint>` against a TestServer. The rules needed to encourage happy-path infrastructure checks while keeping detailed behavior in unit tests.

**What it does:** Enforces use of `Microsoft.AspNetCore.Mvc.Testing`, deterministic and isolated tests, no external service dependencies, and intentional test data seeding/cleanup.

**applyTo:** `src/**/*.IntegrationTests/**/*.cs`

**Example trigger:** You ask Copilot to add an integration test for a new endpoint. Copilot will scaffold a `WebApplicationFactory`-based test that verifies the endpoint wires up and returns the expected status code.

---

### `planning-governance.instructions.md`

**Why:** Large tasks had to follow a plan-first workflow with explicit approval gates before any implementation started. Without enforcement, Copilot would jump straight to code.

**What it does:** Enforces a two-gate workflow: plan approval first, then a separate implementation-start approval. Requires requirements with stable IDs, phased plans with traceability, and task status tracking throughout implementation.

**applyTo:** `**` (all files — planning applies regardless of context)

**Example trigger:** You paste a set of requirements and say "implement this." Copilot will draft a plan and wait for your approval before writing any code.

---

### `troubleshooting-workflow-governance.instructions.md`

**Why:** Debugging sessions without a structured workflow tend to produce scattered changes, no audit trail, and no checkpoints for human review. The troubleshooting workflow requires one iteration at a time with artifacts.

**What it does:** Requires a troubleshooting plan, a dated artifact folder, one iteration file per investigation step, and a stop-and-wait after each iteration before continuing.

**applyTo:** `**` (all files — debugging applies regardless of context)

**Example trigger:** You say "this order calculation is wrong, fix it." Copilot will create an artifact folder, write an analysis file, and stop for your approval before making any code changes.

---

### `documentation-standards.instructions.md`

**Why:** Governance artifacts in `.github/` needed consistent formatting, tone, and discoverability conventions so they remain maintainable over time.

**What it does:** Enforces documentation formatting rules for governance markdown files.

**applyTo:** `.github/**/*.md`

**Example trigger:** You ask Copilot to add a new instruction file. Copilot will follow the existing header, section, and frontmatter conventions.

---

## 3. Prompts

Prompts are `.prompt.md` files in `.github/prompts/`. They are reusable, task-scoped workflows you invoke explicitly by referencing them in chat. Use them when you want Copilot to follow a specific repeatable process rather than improvising.

### `plan-from-requirements.prompt.md`

**Why:** Starting a planning session from scratch is complex. This prompt routes to the Planning Governor agent, captures requirements, and begins the governed workflow.

**How to use:**
> "Use the plan-from-requirements prompt. Here are the requirements: [paste requirements]"

---

### `review-plan-traceability.prompt.md`

**Why:** Before approving a plan, you need confidence that every requirement maps to a task. This prompt runs a traceability check.

**How to use:**
> "Use review-plan-traceability to check the current plan."

---

### `start-implementation-from-current-plan.prompt.md`

**Why:** This is the Gate 2 trigger. After approving a plan, you use this prompt to authorize implementation to begin from the active plan in `memory-bank/current/`.

**How to use:**
> "Use start-implementation-from-current-plan."

---

### `add-endpoint-architecture-safe.prompt.md`

**Why:** Adding a new API endpoint touches multiple layers. This prompt guides Copilot through the full stack (controller → facade → domain service → repository) without violating boundaries.

**How to use:**
> "Use add-endpoint-architecture-safe to add a GET /orders/{id} endpoint."

---

### `add-efcore-repository-method-safely.prompt.md`

**Why:** EF Core queries need specific safety patterns (async, `AsNoTracking`, no N+1, no `IQueryable` leakage). This prompt enforces all of them.

**How to use:**
> "Use add-efcore-repository-method-safely to add a method that retrieves orders by customer ID."

---

### `create-explicit-mapping-functions.prompt.md`

**Why:** Mapping is a common but error-prone task. This prompt ensures hand-written explicit mapping with unit tests.

**How to use:**
> "Use create-explicit-mapping-functions to map OrderEntity to OrderDto."

---

### `add-unit-tests-for-layer.prompt.md`

**Why:** Writing tests is easy to skip or do shallowly. This prompt drives Copilot to cover all required paths for a given layer component.

**How to use:**
> "Use add-unit-tests-for-layer for OrderFacade.CreateOrder."

---

### `add-lightweight-happy-path-integration-tests.prompt.md`

**Why:** Integration tests should verify that endpoints wire up and return correct HTTP responses without duplicating unit test coverage.

**How to use:**
> "Use add-lightweight-happy-path-integration-tests for the orders endpoints."

---

### `review-code-for-architecture-violations.prompt.md`

**Why:** After a large change, it is useful to scan for architecture violations before committing.

**How to use:**
> "Use review-code-for-architecture-violations on the files I just changed."

---

### `review-code-for-test-completeness.prompt.md`

**Why:** Ensures behavioral changes have appropriate tests before a PR is considered done.

**How to use:**
> "Use review-code-for-test-completeness on the changes in OrderFacade."

---

### `investigate-issue-without-changing-code.prompt.md`

**Why:** The first step of troubleshooting is analysis, not code changes. This prompt keeps Copilot in read-only investigation mode.

**How to use:**
> "Use investigate-issue-without-changing-code. The order total is calculating incorrectly."

---

### `perform-one-approved-troubleshooting-iteration.prompt.md`

**Why:** After reviewing an analysis, you approve one iteration at a time. This prompt executes exactly one step and then stops.

**How to use:**
> "Use perform-one-approved-troubleshooting-iteration. Proceed with the fix identified in iteration-001."

---

## 4. Skills

Skills are folders in `.github/skills/`, each containing a `SKILL.md` with detailed domain knowledge and step-by-step procedures. Copilot loads a skill when explicitly instructed to. Skills are more thorough than prompts — they carry richer context for specialized work.

> **Required VS Code setting:** The `planning-governance` and `investigate-bug-troubleshooting-workflow` skills use `context: fork` to run in an isolated subagent. This requires `github.copilot.chat.skillTool.enabled: true` in your VS Code settings.

### `add-api-endpoint`

**Why:** Adding a full endpoint requires coordinating the controller, facade, domain service, repository, DTOs, and tests. A skill-level guide prevents shortcuts.

**How to use:**
> "Use the add-api-endpoint skill to add an endpoint for cancelling an order."

---

### `change-domain-service`

**Why:** Domain services own business rules. Changes here are high-risk and need care to preserve invariants and avoid pushing logic into facades.

**How to use:**
> "Use the change-domain-service skill to add discount calculation logic."

---

### `change-facade`

**Why:** Facades orchestrate multi-service workflows. Changes need to preserve transaction scope, ordering, and error propagation without absorbing business logic.

**How to use:**
> "Use the change-facade skill to add order notification to the checkout flow."

---

### `change-repository`

**Why:** Repository changes interact with EF Core and the database. The skill enforces async patterns, `AsNoTracking`, and safe migration behavior.

**How to use:**
> "Use the change-repository skill to add a method that queries orders by status."

---

### `change-event-handler`

**Why:** Event handlers are the entry point for message queue events. They must stay thin and delegate immediately to facades, just like controllers.

**How to use:**
> "Use the change-event-handler skill to handle the OrderShipped event."

---

### `explicit-mapping`

**Why:** Hand-written mapping is required throughout the project. This skill provides a step-by-step guide for implementing and testing explicit mappers at any layer boundary.

**How to use:**
> "Use the explicit-mapping skill to map CustomerEntity to CustomerDto."

---

### `add-unit-tests`

**Why:** Writing high-quality unit tests requires knowing which paths to cover, how to structure AAA, and how to handle mocking without brittleness.

**How to use:**
> "Use the add-unit-tests skill to add tests for CustomerDomainService.UpdateAddress."

---

### `add-integration-tests-webapplicationfactory`

**Why:** Integration tests using `WebApplicationFactory` have specific setup patterns for test host configuration, fake services, and request execution.

**How to use:**
> "Use the add-integration-tests-webapplicationfactory skill to add a test for the GET /customers endpoint."

---

### `refactor-without-architecture-violations`

**Why:** Refactoring is a high-risk activity for accidental architecture erosion. This skill enforces behavior preservation and boundary checks throughout.

**How to use:**
> "Use the refactor-without-architecture-violations skill to extract the pricing logic from OrderFacade."

---

### `investigate-bug-troubleshooting-workflow`

**Why:** Bug investigation needs a structured, artifact-producing workflow so nothing is lost and each step is reviewable.

**How to use:**
> "Use the investigate-bug-troubleshooting-workflow skill. Customers are seeing duplicate orders."

---

### `planning-governance`

**Why:** Planning is a multi-step process with file lifecycle rules, traceability requirements, and approval gates. A skill-level guide ensures nothing is skipped.

**How to use:**
> "Use the planning-governance skill. I have a new feature to plan."

---

## 5. Agents

Agents are `.agent.md` files in `.github/agents/`. Each agent has a defined role, a set of tools it is allowed to use, and a domain scope. You invoke agents when you want specialist behavior for a focused task.

**Escalation guidance:** Prefer prompts for routine, single-domain work. Prefer skills when you need a step-by-step playbook. Use agents when you need specialist judgment that exceeds what a skill provides, or when you need cross-file review with role-scoped behavior. Each agent has a `## When Not To Use` section — consult it before invoking to avoid unnecessary overhead.

**Two invocation modes:**
- **Direct:** Select the agent in Copilot agent mode and prompt it directly. The agent runs with its specialist role for the full session.
- **Subagent:** Stay in the default agent and ask Copilot to delegate (e.g., `"Invoke the Architecture Guardian agent to review these files for boundary violations."`). Copilot runs the specialist in an isolated context and returns a structured result to the main conversation. Use this when you want a clean review artifact without mixing the specialist review into your implementation thread.

### `architecture-guardian.agent.md` *(user-invocable)*

**Why:** Architecture violation detection needed a dedicated reviewer that focuses only on layer boundaries and dependency direction.

**When not to use:** For routine single-layer edits. Use the `review-code-for-architecture-violations` prompt instead.

**How to use:**
> "@Architecture Guardian review this file for architecture violations."

**As a subagent:**
> "Invoke the Architecture Guardian agent to review `OrderController.cs` and `OrderFacade.cs` for boundary violations."

---

### `dotnet10-csharp-engineer.agent.md` *(user-invocable)*

**Why:** .NET 10 and modern C# have specific patterns for async, nullable, cancellation, and DI. This agent applies those standards precisely.

**When not to use:** For EF Core query performance — that is `EF Core Specialist`. For architecture boundary enforcement — that is `Architecture Guardian`.

**How to use:**
> "@.NET 10 C# Engineer review this service method for async and cancellation issues."

**As a subagent:**
> "Invoke the .NET 10 C# Engineer agent to review `OrderDomainService.cs` for async and cancellation correctness."

---

### `ef-core-specialist.agent.md` *(user-invocable)*

**Why:** EF Core queries need expert attention for tracking, N+1, projection, and migration safety. A specialist agent avoids common pitfalls.

**When not to use:** For routine repository work where the `change-repository` skill or `add-efcore-repository-method-safely` prompt is sufficient.

**How to use:**
> "@EF Core Specialist add a repository query that loads orders with their line items."

**As a subagent:**
> "Invoke the EF Core Specialist agent to design the repository query for loading orders with their line items."

---

### `test-engineer.agent.md` *(user-invocable)*

**Why:** Writing good unit tests is a specialized skill. This agent focuses exclusively on test structure, coverage, and quality.

**When not to use:** For integration test concerns — use the `add-integration-tests-webapplicationfactory` skill. For straightforward single-path unit test additions — use the `add-unit-tests` skill.

**How to use:**
> "@Test Engineer add unit tests for the discount calculation in OrderDomainService."

**As a subagent:**
> "Invoke the Test Engineer agent to assess behavioral coverage gaps for the discount calculation changes in `OrderDomainService`."

---

### `planning-governor.agent.md` *(user-invocable)*

**Why:** Planning is the most complex workflow with strict gate requirements, traceability rules, and file lifecycle management. A dedicated governor agent handles all of it.

**When not to use:** For active implementation tracking after Gate 2 — that is `Planning Coordinator`.

**How to use:**
> "@Planning Governor I have new requirements. [paste requirements]"

**As a subagent:**
> "Invoke the Planning Governor agent with these requirements: [paste requirements]"

---

### `troubleshooting-coordinator.agent.md` *(user-invocable)*

**Why:** Troubleshooting requires controlled iteration with artifacts. This agent runs the investigation workflow and stops after each iteration for approval.

**When not to use:** For single-fix changes where the cause is already known. Use the `perform-one-approved-troubleshooting-iteration` prompt directly.

**How to use:**
> "@Troubleshooting Coordinator the order totals are wrong in production."

**As a subagent:**
> "Invoke the Troubleshooting Coordinator agent on this problem: [description]"

---

### `planning-coordinator.agent.md` *(internal — not user-invocable)*

**Why:** Tracks task status and plan lifecycle moves during active implementation. Used internally by the Planning Governor to manage plan file state. Not meant to be invoked directly.

---

## 6. Hooks

Hooks are in `.github/hooks/`. VS Code loads hook behavior from `.json` files in that directory and executes shell commands at lifecycle points (e.g., `PreToolUse`). The `.md` files are governance checklists — documents the model reads and applies voluntarily. They cannot intercept operations at the platform level.

**Active platform hook:** `git-safety.json` — a real `PreToolUse` hook that intercepts terminal commands and blocks git write operations regardless of how the agent was prompted.

**Governance checklists (documentation):** Located in `.github/checklists/`. Three `.md` files define expected checks for planning pre-conditions, architecture boundaries, and test completeness. These rely on model compliance, not platform interception.

### `pre-implementation-planning-check.md` *(.github/checklists/ — documentation)*

**Why:** Without a hard gate, Copilot would start writing code the moment requirements were pasted. This hook enforces that a plan exists and is approved before any implementation starts.

**When it fires:** Any time implementation is about to begin from a set of requirements.

---

### `git-safety-check.md` + `git-safety.json` *(platform-enforced)*

**Why:** Git write operations by Copilot are prohibited. `git-safety.json` is a real VS Code `PreToolUse` hook that intercepts terminal tool calls and blocks any prohibited git write command at the platform level, regardless of what the model was asked to do. `git-safety-check.md` documents the intent behind the hook.

**When it fires:** Any time a tool call would execute `git add`, `git commit`, `git push`, `git merge`, or similar. The hook script exits with code 2 (blocking error) and the message is shown to the model.

---

### `architecture-boundary-check.md` *(.github/checklists/ — documentation)*

**Why:** Architecture violations are the hardest class of technical debt to reverse. This check runs before code is finalized to confirm no layer boundary has been crossed incorrectly.

**When it fires:** Before completing any implementation that touches multiple layers.

---

### `test-completeness-check.md` *(.github/checklists/ — documentation)*

**Why:** Changes are not complete without tests. This gate prevents marking work done if behavioral changes lack unit or integration test coverage.

**When it fires:** Before any implementation task is considered finished.

---

### `non-blocking-checklists.md` *(.github/checklists/ — non-blocking reminders)*

**Why:** Some checks are important reminders but not hard stops. Collecting them into one document keeps the hook budget low while preserving awareness.

**What it covers:** AutoMapper prohibition reminder, integration test expectation, troubleshooting iteration stop reminder, and plan status update reminder.

---

## 7. Examples

Examples are in `.github/examples/` organized by category. They are concrete code samples showing correct and incorrect patterns that Copilot can reference during code generation and review.

| Category | File | Purpose |
|---|---|---|
| architecture | `correct-controller-to-facade.md` | Controller delegating to facade — correct pattern |
| architecture | `incorrect-controller-to-repository.md` | Controller calling repository directly — prohibited |
| architecture | `correct-event-handler-to-facade.md` | Event handler delegating to facade — correct pattern |
| architecture | `correct-facade-orchestration.md` | Facade coordinating services — correct pattern |
| architecture | `correct-domain-service-behavior.md` | Domain service owning business rules — correct pattern |
| architecture | `correct-repository-behavior.md` | Repository owning persistence — correct pattern |
| mapping | `correct-explicit-mapper.md` | Hand-written explicit mapping — correct pattern |
| mapping | `incorrect-automapper-usage.md` | AutoMapper usage — prohibited |
| testing | `correct-unit-test-structure.md` | AAA unit test structure — correct pattern |
| testing | `correct-webapplicationfactory-integration-test.md` | Integration test setup — correct pattern |
| planning | `correct-plan-format.md` | Plan document format with traceability |
| troubleshooting | `correct-troubleshooting-artifact-format.md` | Iteration artifact format |

**How to use:** Reference an example in chat when you want Copilot to follow a specific pattern:
> "Follow the pattern in correct-controller-to-facade when adding this endpoint."

---

## 8. Governance Activation Profile

### `.github/governance-activation-profile.md`

**Why:** With many governance artifacts available, it was important to document which mechanism to reach for first and how to avoid activating too many hooks or agents unnecessarily.

**What it does:** Ranks governance mechanisms by priority (instructions first, then prompts/skills, then agents, then hooks), defines the hook budget, lists primary vs. specialized prompts, and documents anti-patterns to avoid.

**How to use:** Consult this file when adding new governance artifacts or when evaluating whether the governance model is generating unnecessary overhead. It is the tuning reference for the governance system itself.
