---
description: "Strict git safety policy for Copilot operations."
name: "Git Safety Governance"
applyTo: "**"
---
# Git Safety Governance

## Allowed Read-Only Commands
- `git status`
- `git diff`
- `git log`
- `git show`
- `git branch --show-current`
- Other read-only inspection commands

## Prohibited Write Commands
- `git add`, `git commit`, `git push`, `git pull`
- `git merge`, `git rebase`
- `git checkout`, `git switch`
- `git reset`, `git clean`, `git stash`
- Branch create, delete, and rename operations

## Required Behavior
- If write behavior is required, stop and ask the user to perform it manually.
