# Project Governance Instructions

## Scope
These rules apply to all Copilot-assisted work in this repository.

## Source Of Truth
- If requirement text conflicts with repository conventions, repository code and docs are authoritative.
- Architecture reference: `docs/ServiceTierResponsibilities.png` and `docs/ServiceDiagram.md`.

## Architecture Boundaries
- Request flow: HTTP Request -> Controller -> Facade -> Domain Service -> Repository -> Database.
- Event flow: Message Queue -> Event Handler -> Facade -> Domain Service -> Repository -> Database.
- Controllers and event handlers delegate orchestration to facades.
- Business logic belongs in domain entities, domain services, and facades according to existing patterns.
- Repositories encapsulate persistence and EF details.

## Mapping Rules
- Explicit mapping only.
- AutoMapper and reflection-based mapping are prohibited.
- Keep mapping responsibilities at layer boundaries.

## Testing Rules
- Changes are not complete without appropriate tests.
- Add or update unit tests for behavioral changes.
- Add or update integration tests when API or pipeline behavior is affected.
- Prefer existing test projects near modified layers.

## Git Safety
- Allowed: read-only inspection (`git status`, `git diff`, `git log`, `git show`, `git branch --show-current`).
- Prohibited: all write and state-changing git operations (`add`, `commit`, `push`, `pull`, `merge`, `rebase`, `checkout`, `switch`, `reset`, `clean`, `stash`, branch create/delete).
- If a task requires git write actions, stop and ask the user to perform them.

## Planning And Troubleshooting Gates
- Planning is mandatory before implementation when requirements are provided.
- Plan approval allows moving a plan to current state only.
- Implementation starts only after explicit second approval.
- Troubleshooting must run one approved iteration at a time with artifact notes per iteration.

## Governance Mechanism Preference
- Put global standards in instruction files first.
- Use prompts and skills for repeatable workflows.
- Use agents for specialized tasks with explicit role boundaries.
- Keep blocking hooks limited to critical gates (planning pre-check, git safety, architecture boundaries, and test completeness).
