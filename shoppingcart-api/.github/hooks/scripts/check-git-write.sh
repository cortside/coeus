#!/usr/bin/env bash
# check-git-write.sh
#
# VS Code PreToolUse hook that blocks prohibited git write operations.
# Receives a JSON object via stdin with tool_name and tool_input fields.
#
# Exit code 2 = blocking error; message on stderr is shown to the model.
# Exit code 0 = allow the tool call to proceed.

INPUT=$(cat)

COMMAND=$(echo "$INPUT" | python3 -c "
import sys, json
try:
    d = json.load(sys.stdin)
    ti = d.get('tool_input', {})
    print(ti.get('command', '') or ti.get('cmd', ''))
except Exception:
    print('')
" 2>/dev/null || echo "")

if echo "$COMMAND" | grep -qiE '^\s*git\s+(add|commit|push|pull|merge|rebase|checkout|switch|reset|clean|stash)\b'; then
    echo "Git write operation blocked by governance policy. Ask the user to perform this operation manually in the terminal." >&2
    exit 2
fi

exit 0
