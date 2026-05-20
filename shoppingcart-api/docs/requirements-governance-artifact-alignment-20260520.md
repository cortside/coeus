# Requirements: Governance Artifact Alignment

## Metadata
- Date: 2026-05-20
- Source: User request and original governance generation requirements
- Owner: Copilot

## Requirement Catalog
- REQ-001: Review all governance artifacts under .github and related docs for correct role separation and intended usage of instructions, prompts, skills, agents, hooks, and examples.
- REQ-002: Use repository content and conventions as source of truth when any discrepancy exists between requirement text and current repository patterns.
- REQ-003: Adjust governance file content where needed so each mechanism has a clear purpose and avoids overlap.
- REQ-004: Ensure instructions remain policy-first, broadly discoverable, and properly scoped by applyTo without unnecessary activation noise.
- REQ-005: Ensure prompts are concise, reusable workflow launchers and not duplicated deep playbooks.
- REQ-006: Ensure skills are deeper procedural playbooks that add value beyond matching prompts.
- REQ-007: Ensure agents are role-specialized, clearly invocable where appropriate, and not overlapping in ownership.
- REQ-008: Ensure hooks stay high-signal with a small blocking set and explicit non-blocking guidance for secondary checks.
- REQ-009: Ensure examples remain accurate and aligned to architecture, mapping, testing, planning, and troubleshooting governance.
- REQ-010: Ensure governance readmes and docs make the system easy for new engineers to understand and adopt.
- REQ-011: Preserve architecture boundaries and mapping/testing/git/planning/troubleshooting guardrails already established.
- REQ-012: Do not change application runtime behavior while performing governance updates.
- REQ-013: Do not perform git write operations.
- REQ-014: Provide a final implementation summary describing files changed, assumptions, and recommendations.

## Non-Functional Requirements
- REQ-NF-001: Keep edits minimal, focused, and internally consistent across files.
- REQ-NF-002: Prioritize readability and beginner-friendly clarity in documentation surfaces.
- REQ-NF-003: Maintain deterministic naming and discoverability across index/readme files.

## Assumptions
- Existing governance files are a valid baseline and require refinement, not full replacement.
- Existing project architecture and conventions reflected in repository code/docs are authoritative.

## Constraints
- Planning governance gates must be followed before implementation edits.
- Git write/state-changing operations are prohibited.
- Scope is governance/docs content, not product code.

## Open Questions
- Whether to enforce explicit prompt-to-skill linkage conventions in frontmatter or keep linkage as documentation-only.
- Whether any currently user-invocable skill should be narrowed to reduce overlap with prompts.
