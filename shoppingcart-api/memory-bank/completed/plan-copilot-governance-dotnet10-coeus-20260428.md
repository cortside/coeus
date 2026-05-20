# Plan: Copilot Governance for .NET 10 Coeus-Style Service

- Owner: Copilot
- Date: 2026-04-28
- Status: Complete
- Related Requirements: `docs/requirements-copilot-governance-dotnet10-coeus-20260428.md`
- Scope: Governance artifact generation only

## Approval Checkpoints

- Gate 1: Plan Approval Required
- Gate 2: Implementation Start Approval Required

Implementation must not begin until both gates are explicitly satisfied in sequence.

## Requirement Catalog (Normalized)

- REQ-001 to REQ-003: Generate complete governance using repository as source of truth.
- REQ-004 to REQ-012: Enforce Coeus architecture boundaries and tier responsibilities.
- REQ-013 to REQ-016: Enforce .NET 10/C# quality, async/cancellation, DI, exceptions, and safe logging.
- REQ-017 to REQ-018: Enforce ASP.NET Core API/controller and model-shape boundaries.
- REQ-019 to REQ-022: Enforce EF Core boundary and query/persistence best practices.
- REQ-023 to REQ-025: Enforce explicit mapping and AutoMapper prohibition with testability.
- REQ-026 to REQ-029: Enforce unit and integration testing governance.
- REQ-030 to REQ-031: Enforce strict git safety constraints.
- REQ-032 to REQ-035: Enforce planning lifecycle and approval-gated execution.
- REQ-036 to REQ-037: Enforce troubleshooting iteration workflow with artifact trail.
- REQ-038 to REQ-046: Produce required instruction/skill/prompt/agent/hook/example artifacts and completion reporting.

## Assumptions And Constraints

- Repository architecture and conventions are authoritative when requirement text and codebase details differ.
- Governance artifacts are primarily markdown-based under `.github/` and related documentation folders.
- No application runtime behavior changes are permitted during governance artifact creation.
- No git write/state-changing commands are permitted.

## Phased Plan

### Phase 1 - Baseline And Governance Skeleton

- Objective: Establish discoverable governance structure and indexing aligned to existing repository conventions.
- In-scope tasks:
  - Inventory existing `.github/` governance assets and gaps.
  - Add/update discoverability docs for governance navigation.
  - Define canonical naming and placement standards for governance files.
- Out-of-scope:
  - Any runtime code or test behavior changes.
  - Any git write operations.
- Dependencies and risks:
  - Risk: Existing partially overlapping governance files may conflict in guidance.
  - Mitigation: Consolidate references and preserve strictest repository-safe rules.
- Exit criteria:
  - Governance index/readme surfaces all categories and entry points.
  - File/folder naming and ownership conventions documented.

### Phase 2 - Instruction Governance Set

- Objective: Produce instruction files that codify architecture, coding standards, testing, planning, troubleshooting, and safety boundaries.
- In-scope tasks:
  - Create/align instruction files for all required categories.
  - Encode prohibited behaviors and stop conditions.
  - Add repository-specific boundary examples in instruction text.
- Out-of-scope:
  - Implementing new application features.
- Dependencies and risks:
  - Dependency: Clear mapping to architecture diagram and current layering.
  - Risk: Overly generic instruction content not aligned to repository conventions.
  - Mitigation: Reference concrete project structure and scripts.
- Exit criteria:
  - All required instruction categories exist and are discoverable.
  - Rules include architecture, mapping, test, git, planning, and troubleshooting controls.

### Phase 3 - Skills, Prompts, And Agent Governance

- Objective: Provide reusable operational workflows via skills/prompts/agents that enforce boundaries and stop conditions.
- In-scope tasks:
  - Create required skill files.
  - Create required prompt files.
  - Create required agent files with purpose, checks, prohibited actions, output format, and stop conditions.
- Out-of-scope:
  - Running implementation tasks from those prompts/agents.
- Dependencies and risks:
  - Risk: Inconsistent assumptions across prompts/skills/agents.
  - Mitigation: Standardize shared constraints (architecture, mapping, tests, git, planning gates).
- Exit criteria:
  - All required skills/prompts/agents are present and cross-referenced.
  - Stop conditions explicitly enforced for planning and troubleshooting workflows.

### Phase 4 - Hooks, Examples, And Validation

- Objective: Add policy hooks (markdown governance if needed), reference examples, and completeness validation.
- In-scope tasks:
  - Create hook governance files for all required checks.
  - Add correct/incorrect examples for architecture, mapping, tests, planning, troubleshooting.
  - Validate full requirement-to-artifact coverage.
- Out-of-scope:
  - Enforcing executable hooks in CI if unsupported.
- Dependencies and risks:
  - Dependency: Finalized instruction/prompt/skill/agent language.
  - Risk: Missing coverage for one or more mandatory example/hook categories.
  - Mitigation: Use traceability matrix and checklist validation task.
- Exit criteria:
  - All required hook and example categories present.
  - Coverage validation confirms each requirement is addressed by one or more tasks/artifacts.

## Task List With Requirement Traceability

- [x] TASK-001
  - Description: Inventory existing governance files and repository conventions for architecture, testing, scripts, and safety constraints.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-001, REQ-002, REQ-003
  - Target Files/Components: `.github/`, `docs/ServiceDiagram.md`, `docs/ServiceTierResponsibilities.png`, `README.md`, `SCRIPTS.md`
  - Acceptance Criteria: Inventory notes identify existing assets, gaps, and conflicting guidance.
  - Test Expectations: Manual verification that inventory includes all required governance categories.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-002
  - Description: Create/update governance discoverability index and category map.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-003, REQ-038, REQ-045
  - Target Files/Components: `.github/README.md` or `.github/governance-index.md`
  - Acceptance Criteria: Index links every governance category and explains usage.
  - Test Expectations: Link/path checks for all referenced governance files.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-003
  - Description: Author global repository behavior instruction file aligning with project architecture and repository safety constraints.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-001, REQ-002, REQ-004, REQ-030, REQ-046
  - Target Files/Components: `.github/instructions/global-repository-governance.instructions.md`
  - Acceptance Criteria: File codifies allowed/prohibited actions, layering expectations, and source-of-truth precedence.
  - Test Expectations: Manual review for alignment with existing repository conventions.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-004
  - Description: Author .NET 10/C# coding standards instruction file.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-013, REQ-014, REQ-015, REQ-016, REQ-038
  - Target Files/Components: `.github/instructions/dotnet10-csharp-standards.instructions.md`
  - Acceptance Criteria: Rules include async patterns, cancellation, DI, exceptions, and sensitive logging constraints.
  - Test Expectations: Checklist review against requirement catalog.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-005
  - Description: Author ASP.NET Core MVC/API governance instruction file.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-007, REQ-017, REQ-018, REQ-038
  - Target Files/Components: `.github/instructions/aspnetcore-api-governance.instructions.md`
  - Acceptance Criteria: Thin-controller policy and transport/model-boundary requirements are explicit.
  - Test Expectations: Manual policy verification against existing WebApi project conventions.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-006
  - Description: Author architecture/layering instruction file covering request/event flow responsibilities by tier.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-004, REQ-005, REQ-006, REQ-008, REQ-009, REQ-010, REQ-011, REQ-012, REQ-038
  - Target Files/Components: `.github/instructions/architecture-layering-governance.instructions.md`
  - Acceptance Criteria: Tier responsibilities and prohibited cross-layer calls are explicit and repository-aligned.
  - Test Expectations: Scenario checklist includes controller/event-handler/facade/domain-service/repository/entity interactions.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-007
  - Description: Author EF Core governance instruction file.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-019, REQ-020, REQ-021, REQ-022, REQ-038
  - Target Files/Components: `.github/instructions/efcore-governance.instructions.md`
  - Acceptance Criteria: Query shape, async/cancellation, tracking strategy, and migration constraints are explicit.
  - Test Expectations: Policy checklist review for required EF guidance.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-008
  - Description: Author mapping governance instruction file with explicit mapping and AutoMapper prohibition.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-023, REQ-024, REQ-025, REQ-038
  - Target Files/Components: `.github/instructions/mapping-governance.instructions.md`
  - Acceptance Criteria: Bans AutoMapper/implicit mapping; defines layer-specific mapping responsibilities and testing expectations.
  - Test Expectations: Manual review for explicit prohibition language and mapping test requirements.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-009
  - Description: Author unit testing governance instruction file.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-026, REQ-027, REQ-038, REQ-044
  - Target Files/Components: `.github/instructions/unit-testing-governance.instructions.md`
  - Acceptance Criteria: Layer coverage and scenario expectations are codified.
  - Test Expectations: Checklist confirms success/failure/edge/mapping/validation coverage requirements.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-010
  - Description: Author integration testing governance instruction file with `WebApplicationFactory` guidance.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-028, REQ-029, REQ-038, REQ-044
  - Target Files/Components: `.github/instructions/integration-testing-governance.instructions.md`
  - Acceptance Criteria: Guidance enforces deterministic, isolated, happy-path-focused integration tests.
  - Test Expectations: Checklist confirms required framework and isolation guidance.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-011
  - Description: Author git safety instruction file with allowed and prohibited command sets.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-030, REQ-031, REQ-038, REQ-046
  - Target Files/Components: `.github/instructions/git-safety-governance.instructions.md`
  - Acceptance Criteria: Explicit read-only allowed list and disallowed write/state-changing operations are documented.
  - Test Expectations: Review against requirements allowed/prohibited command lists.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-012
  - Description: Author planning workflow instruction file with two-step approval gate.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-032, REQ-033, REQ-034, REQ-035, REQ-038
  - Target Files/Components: `.github/instructions/planning-workflow-governance.instructions.md`
  - Acceptance Criteria: Distinguishes plan approval from implementation start approval and plan lifecycle moves.
  - Test Expectations: Simulated workflow walkthrough validates stop conditions.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-013
  - Description: Author troubleshooting workflow instruction file with artifact iteration stop rules.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-036, REQ-037, REQ-038
  - Target Files/Components: `.github/instructions/troubleshooting-workflow-governance.instructions.md`
  - Acceptance Criteria: Timestamped artifact folder and per-iteration stop/approval model are explicit.
  - Test Expectations: Workflow checklist validates naming and required iteration-note fields.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-014
  - Description: Author documentation standards instruction file for governance consistency and discoverability.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-003, REQ-038, REQ-045
  - Target Files/Components: `.github/instructions/documentation-standards.instructions.md`
  - Acceptance Criteria: Defines required sections, naming patterns, and cross-linking expectations.
  - Test Expectations: Manual review of section/template conformance.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-015
  - Description: Create skill files for endpoint/domain service/facade/repository/event handler/mapping/unit-test/integration-test/refactor/troubleshoot/planning workflows.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-039
  - Target Files/Components: `.github/skills/**`
  - Acceptance Criteria: All required skills exist with clear usage boundaries and required checks.
  - Test Expectations: Skill inventory checklist confirms all required topics are covered.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-016
  - Description: Create reusable prompt files for planning, review, implementation start, architecture-safe development, testing, mapping, troubleshooting, and code review.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-040
  - Target Files/Components: `.github/prompts/**`
  - Acceptance Criteria: All required prompt categories exist and encode architecture/test/safety expectations.
  - Test Expectations: Prompt inventory checklist and frontmatter validation.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-017
  - Description: Create agent files for Architecture Guardian, .NET 10 C# Engineer, EF Core Specialist, API Specialist, testing roles, mapping, planning, troubleshooting, and git safety.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-041
  - Target Files/Components: `.github/agents/**`
  - Acceptance Criteria: Each agent includes purpose, when-to-use, expected inputs, required checks, prohibited actions, output format, and stop conditions.
  - Test Expectations: Agent schema checklist confirms required sections for each agent.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-018
  - Description: Create hook governance files for required checks (planning, git, architecture, tests, mapping prohibition, integration expectations, troubleshooting stop, plan status).
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-042
  - Target Files/Components: `.github/hooks/**`
  - Acceptance Criteria: All required hook policies are represented, executable or markdown-governed.
  - Test Expectations: Hook checklist confirms each required check exists.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-019
  - Description: Create example files for correct/incorrect architecture, mapping, testing, planning, and troubleshooting patterns.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-043
  - Target Files/Components: `.github/examples/**`
  - Acceptance Criteria: All required example categories exist and are clearly labeled as correct/incorrect.
  - Test Expectations: Example inventory checklist validates each required scenario is covered.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-020
  - Description: Add anti-AutoMapper and explicit-mapping cross-references across instructions, skills, prompts, and examples.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-023, REQ-024, REQ-025, REQ-043, REQ-044
  - Target Files/Components: `.github/instructions/**`, `.github/skills/**`, `.github/prompts/**`, `.github/examples/**`
  - Acceptance Criteria: No governance artifact recommends AutoMapper or implicit mapping.
  - Test Expectations: Repository search confirms AutoMapper only appears in prohibition contexts.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-021
  - Description: Add architecture-boundary cross-references across instructions, prompts, skills, and agents.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-004, REQ-005, REQ-006, REQ-007, REQ-008, REQ-009, REQ-010, REQ-011, REQ-012, REQ-017, REQ-018, REQ-044
  - Target Files/Components: `.github/instructions/**`, `.github/prompts/**`, `.github/skills/**`, `.github/agents/**`
  - Acceptance Criteria: Boundary rules are consistent and non-conflicting across all governance surfaces.
  - Test Expectations: Manual consistency review and spot-check scenarios.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-022
  - Description: Add testing completion and validation requirements across governance content.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-026, REQ-027, REQ-028, REQ-029, REQ-044
  - Target Files/Components: `.github/instructions/**`, `.github/prompts/**`, `.github/skills/**`, `.github/hooks/**`
  - Acceptance Criteria: Completion definitions explicitly require appropriate tests and passing status.
  - Test Expectations: Checklist confirms testing requirements are present in each relevant artifact category.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-023
  - Description: Add planning and troubleshooting gate/stop-condition consistency checks across prompts, agents, and hooks.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-032, REQ-033, REQ-034, REQ-035, REQ-036, REQ-037, REQ-042, REQ-044
  - Target Files/Components: `.github/prompts/**`, `.github/agents/**`, `.github/hooks/**`
  - Acceptance Criteria: Plan/implementation and troubleshooting iteration approvals are explicit and consistent.
  - Test Expectations: Simulated workflow review for both planning and troubleshooting scenarios.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-024
  - Description: Validate full requirement-to-task and requirement-to-artifact traceability and close any gaps.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-001 through REQ-046
  - Target Files/Components: Plan and all generated governance artifacts
  - Acceptance Criteria: Every requirement maps to at least one task and at least one final artifact.
  - Test Expectations: Traceability matrix and completion checklist pass with no orphaned requirements.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-025
  - Description: Produce final governance-generation summary with files created, coverage categories, assumptions, discovered conventions, and recommendations.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-045
  - Target Files/Components: Delivery summary response and optional governance summary doc
  - Acceptance Criteria: Summary includes all five required reporting areas.
  - Test Expectations: Manual verification against reporting requirement.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

- [x] TASK-026
  - Description: Verify no git write operations were performed and that no unrelated refactors/application behavior changes were introduced.
  - Owner: Copilot
  - Status: Done
  - Linked Requirements: REQ-046, REQ-030, REQ-031
  - Target Files/Components: Session command log and changed-file review
  - Acceptance Criteria: Evidence shows compliance with safety constraints.
  - Test Expectations: Manual command audit and changed-file scope review.
  - Completion Notes: Completed on 2026-04-28 during governance implementation.

## Requirement To Task Traceability Matrix

| Requirement ID | Task IDs |
|---|---|
| REQ-001 | TASK-001, TASK-003, TASK-024 |
| REQ-002 | TASK-001, TASK-003 |
| REQ-003 | TASK-001, TASK-002, TASK-014 |
| REQ-004 | TASK-006, TASK-021 |
| REQ-005 | TASK-006, TASK-021 |
| REQ-006 | TASK-006, TASK-021 |
| REQ-007 | TASK-005, TASK-006, TASK-021 |
| REQ-008 | TASK-006, TASK-021 |
| REQ-009 | TASK-006, TASK-021 |
| REQ-010 | TASK-006, TASK-021 |
| REQ-011 | TASK-006, TASK-007, TASK-021 |
| REQ-012 | TASK-006, TASK-021 |
| REQ-013 | TASK-004 |
| REQ-014 | TASK-004, TASK-007 |
| REQ-015 | TASK-004 |
| REQ-016 | TASK-004 |
| REQ-017 | TASK-005, TASK-021 |
| REQ-018 | TASK-005, TASK-006, TASK-021 |
| REQ-019 | TASK-007 |
| REQ-020 | TASK-007 |
| REQ-021 | TASK-007 |
| REQ-022 | TASK-007 |
| REQ-023 | TASK-008, TASK-020 |
| REQ-024 | TASK-008, TASK-020 |
| REQ-025 | TASK-008, TASK-022 |
| REQ-026 | TASK-009, TASK-010, TASK-022 |
| REQ-027 | TASK-009, TASK-022 |
| REQ-028 | TASK-010, TASK-022 |
| REQ-029 | TASK-010, TASK-022 |
| REQ-030 | TASK-011, TASK-026 |
| REQ-031 | TASK-011, TASK-026 |
| REQ-032 | TASK-012, TASK-023 |
| REQ-033 | TASK-012, TASK-023 |
| REQ-034 | TASK-012, TASK-023 |
| REQ-035 | TASK-012, TASK-023 |
| REQ-036 | TASK-013, TASK-023 |
| REQ-037 | TASK-013, TASK-023 |
| REQ-038 | TASK-002, TASK-003, TASK-004, TASK-005, TASK-006, TASK-007, TASK-008, TASK-009, TASK-010, TASK-011, TASK-012, TASK-013, TASK-014 |
| REQ-039 | TASK-015 |
| REQ-040 | TASK-016 |
| REQ-041 | TASK-017 |
| REQ-042 | TASK-018 |
| REQ-043 | TASK-019 |
| REQ-044 | TASK-009, TASK-010, TASK-020, TASK-021, TASK-022, TASK-023 |
| REQ-045 | TASK-002, TASK-025 |
| REQ-046 | TASK-003, TASK-011, TASK-026 |

## Progress Log

- 2026-04-28: Plan drafted from normalized requirements. No implementation started. Awaiting Gate 1 plan approval.
- 2026-04-28: Gate 1 approved and plan moved from `memory-bank/planning/` to `memory-bank/current/`.
- 2026-04-28: Gate 2 implementation-start approval received. Governance artifacts generated under `.github/` and supporting docs.
- 2026-04-28: Task statuses, completion notes, and traceability validation updated. Awaiting user verification to close plan.
- 2026-04-28: Governance quality audit completed; redundant and low-value blocking hooks were consolidated into a low-overhead hook model with non-blocking checklists.
- 2026-04-28: Activation-realism pass completed; agent visibility, prompt/skill prioritization, and mechanism selection guidance were tuned for practical day-to-day usage.
- 2026-04-28: Instruction scope pass completed; API governance was expanded to both API projects and EF Core governance was narrowed to data-layer files to improve signal-to-noise.
- 2026-04-28: Final precision pass on architecture-layering and mapping governance scopes; both confirmed correct at `src/**/*.cs` — no changes required. Plan complete.
- 2026-05-20: User verified completion. Plan moved to `memory-bank/completed/`.

## Completion Verification Checklist

- [x] Gate 1 approved by user.
- [x] Plan moved to `memory-bank/current/` after Gate 1 approval.
- [x] Gate 2 implementation-start approval explicitly provided by user.
- [x] All tasks updated with final status and completion notes.
- [x] Requirement-to-artifact traceability validated.
- [x] Final summary delivered with required reporting fields.
- [x] Plan moved to `memory-bank/completed/` after user verifies completion.
