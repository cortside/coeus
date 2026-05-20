# Copilot Governance One Page

Use this page as a quick desk reference.

## 1) Ask In This Format

Outcome: what should exist when done.
Scope: files, layers, or components involved.
Constraints: architecture boundaries, explicit mapping, testing expectations.
Done means: verification criteria.

## 2) Choose The Right Tool

1. Plain request: routine and clear task.
2. Prompt: repeatable process.
3. Skill: deeper implementation playbook.
4. Agent: high-risk, cross-layer, or ambiguous work.

## 3) Safety Non-Negotiables

1. No git write commands by Copilot.
2. Keep Controller -> Facade -> Domain Service -> Repository boundaries.
3. Use explicit mapping only (no AutoMapper).
4. Behavior changes require test updates.
5. Planning and troubleshooting follow approval gates.
6. Do not include secrets, connection strings, payment data, or auth tokens in Copilot prompts. AI-generated changes to auth, payment, or secrets handling require mandatory human review.

## 4) Fast Copy-Paste Requests

Plan from requirements:
Use plan-from-requirements. Here are the requirements: <paste requirements>.

Add endpoint safely:
Use add-endpoint-architecture-safe to add GET /orders/{id}.

Check architecture:
Use review-code-for-architecture-violations on my changed files.

Check tests:
Use review-code-for-test-completeness on my changed files.

Specialist test review:
Use Test Engineer to assess unit test readiness for these changes: <summary>.

## 5) What Good Looks Like

1. Thin controllers and handlers.
2. Business logic in domain service or facade where appropriate.
3. Explicit mapping at boundaries.
4. Updated unit and integration tests where behavior changed.
5. No prohibited git operations.

## 6) New Team Rollout

1. Week 1: plain requests plus core prompts.
2. Week 2: skills for common implementation tasks.
3. Week 3: agent escalation for high-risk work.
4. Week 4: tune usage with governance activation profile.

## 7) Canonical References

- [Copilot Governance Guide](CopilotGovernanceGuide.md)
- [Beginner Quickstart](CopilotGovernanceBeginnerQuickstart.md)
- [Governance Index](../.github/README.md)
- [Instructions Index](../.github/instructions/README.md)
- [Prompts Index](../.github/prompts/README.md)
- [Skills Index](../.github/skills/README.md)
- [Agents Index](../.github/agents/README.md)
