# Requirements Catalog: Copilot Governance for .NET 10 Coeus-Style Service

- Date: 2026-04-28
- Source: User-provided governance requirements and repository source of truth
- Scope: Governance content only (no application behavior changes)

## Normalized Requirement Catalog

- REQ-001: Generate a complete GitHub Copilot governance system for this existing .NET 10 repository under `.github/` and supporting folders.
- REQ-002: Repository content is the source of truth when any discrepancy exists between requirements text and implementation details.
- REQ-003: Governance generation must include instructions, prompts, skills, agents, hooks, examples, and supporting discoverability documentation.

### Architecture Governance
- REQ-004: Enforce request flow boundary: `HTTP Request -> Controller -> Facade -> Domain Service -> Repository -> Database`.
- REQ-005: Enforce event flow boundary: `Message Queue -> Event Handler -> Facade -> Domain Service -> Repository -> Database`.
- REQ-006: Enforce relationship boundaries: `Repository <-> Domain Entity` and `Domain Service -> Domain Entity`.
- REQ-007: Controllers must remain thin, perform request/response mapping and boundary concerns, delegate to facades, and avoid business logic, repository access, and direct EF Core use.
- REQ-008: Event handlers must process broker contracts, validate/deserialize/map input, delegate to facades, and avoid business logic and direct repository/EF Core access.
- REQ-009: Facades must orchestrate multi-service workflows, manage transactions/unit-of-work where needed, map to upper-layer DTOs, and avoid transport-specific concerns.
- REQ-010: Domain services must enforce cross-entity/domain rules, use repositories/approved abstractions, and avoid HTTP/controller concerns.
- REQ-011: Repositories must encapsulate persistence and EF queries, map persistence/domain shapes as needed, and avoid business logic and upward-layer dependencies.
- REQ-012: Domain entities must protect invariants and avoid DI/infrastructure/controller concerns.

### .NET 10 and C# Governance
- REQ-013: Enforce modern .NET 10/C# practices: nullable correctness, readable code, cohesive methods, async/await for I/O, no sync-over-async, DI/constructor injection, no service locator, no static mutable state.
- REQ-014: Enforce use of `CancellationToken` on async public controller/handler/service/repository APIs where appropriate.
- REQ-015: Enforce purposeful exception handling and consistent domain-specific exception/result patterns with existing project style.
- REQ-016: Enforce structured logging and prohibit logging secrets/tokens/passwords/PII/sensitive business data.

### ASP.NET Core Governance
- REQ-017: Enforce thin controller design, proper status codes, consistent model validation/routing conventions, and existing API versioning/problem-details/OpenAPI conventions where present.
- REQ-018: Preserve separation among request DTOs, response DTOs, internal DTOs, domain entities, and EF entities.

### EF Core Governance
- REQ-019: Constrain DbContext usage to infrastructure/repository patterns unless existing architecture explicitly differs.
- REQ-020: Enforce async EF APIs with cancellation, intentional includes/projections/tracking behavior, `AsNoTracking()` for read-only, avoidance of N+1 and unintended client-side evaluation.
- REQ-021: Do not expose `IQueryable` beyond approved boundaries unless explicitly permitted by existing architecture.
- REQ-022: Preserve migrations/concurrency/audit/soft-delete conventions and do not create migrations unless explicitly requested.

### Mapping Governance
- REQ-023: Prohibit AutoMapper and reflection/implicit generic mapping utilities.
- REQ-024: Require explicit, readable, layer-appropriate mapping responsibilities across controllers, handlers, facades, domain services, and repositories.
- REQ-025: Require mapping behavior tests for meaningful defaults/computed fields/enums/nullability/nested structures.

### Testing Governance
- REQ-026: Enforce mandatory testing: work is not complete without appropriate unit and integration test updates.
- REQ-027: Unit testing governance must cover relevant layers, clear Arrange/Act/Assert style, happy/edge/failure/validation/authorization/mapping paths, and avoid brittle implementation-coupled tests.
- REQ-028: Integration testing governance must align with ASP.NET Core guidance using `Microsoft.AspNetCore.Mvc.Testing` and `WebApplicationFactory<TEntryPoint>`, focused on important pipeline/infrastructure behavior.
- REQ-029: Integration tests must remain deterministic and isolated, avoid external service dependence, and use approved doubles/fakes/containers/local alternatives.

### Git Safety Governance
- REQ-030: Prohibit git write/state-changing commands and allow read-only git inspection commands only.
- REQ-031: If work requires git write actions, Copilot must stop and ask the user to perform those actions manually.

### Planning Governance
- REQ-032: Enforce plan-first workflow: copy requirements into `docs/`, create phased plan in `memory-bank/planning/`, include requirement/task traceability, statuses, explicit testing/docs/validation tasks, then stop.
- REQ-033: Distinguish approvals: plan approval allows move to `memory-bank/current/` only; implementation requires a second explicit approval.
- REQ-034: During implementation, maintain live task status/traceability/completion tracking and do not mark complete without meeting tests and acceptance criteria.
- REQ-035: On user-verified completion, mark plan complete and move from `memory-bank/current/` to `memory-bank/completed/`.

### Troubleshooting Governance
- REQ-036: For fix/debug/investigate requests, require troubleshooting plan first and creation of timestamped `artifacts/YYYYMMDD-HHMMSS-short-description/` folder.
- REQ-037: For each troubleshooting iteration, create a new notes file documenting inspection, changes, rationale, tests, results, and next step; stop after each iteration for user approval.

### Required Governance Deliverables and Completion Rules
- REQ-038: Generate instruction files for global behavior, coding standards, MVC/API, EF Core, architecture/layering, mapping, testing/integration, git safety, planning, troubleshooting, and documentation standards.
- REQ-039: Generate skills for endpoint/domain service/facade/repository/event handler/mapping/unit tests/integration tests/refactoring/troubleshooting/planning workflows.
- REQ-040: Generate reusable prompts for planning, plan review, implementation start, architecture-safe endpoint/repository/test/mapping/troubleshooting/code-review workflows.
- REQ-041: Generate agents for architecture, C#, EF Core, API, testing, integration testing, mapping, planning, troubleshooting, and git safety; each must include purpose, usage, inputs, checks, prohibited actions, output format, and stop conditions.
- REQ-042: Generate hook governance (or markdown equivalents) for planning check, git safety, architecture boundary, test completeness, mapping prohibition, integration expectations, troubleshooting stop, and plan status updates.
- REQ-043: Generate examples for correct and incorrect patterns across architecture, mapping (including AutoMapper prohibition), testing, planning, and troubleshooting artifacts.
- REQ-044: Enforce completion gates for coding/planning/troubleshooting, including architecture boundaries, explicit mapping, test expectations, and no git write operations.
- REQ-045: After generation, provide summary of files created, governance categories covered, assumptions, discovered conventions, and follow-up recommendations.
- REQ-046: Do not perform git write operations, unrelated refactoring, or application behavior changes while generating governance.

## Assumptions

- A1: The governance implementation phase will create or update files under `.github/`, `docs/`, and possibly `memory-bank/` without altering runtime service behavior.
- A2: Existing repository conventions in architecture and naming are authoritative where any ambiguity appears.
- A3: Markdown-based hooks are acceptable when executable hook infrastructure is unavailable.

## Out Of Scope For This Planning Step

- Creating governance implementation files.
- Modifying application code, tests, or runtime behavior.
- Performing any git write operation.
