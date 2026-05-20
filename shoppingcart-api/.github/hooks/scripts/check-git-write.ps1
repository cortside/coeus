# check-git-write.ps1
#
# VS Code PreToolUse hook that blocks prohibited git write operations.
# Receives a JSON object via stdin with tool_name and tool_input fields.
#
# Exit code 2 = blocking error; message on stderr is shown to the model.
# Exit code 0 = allow the tool call to proceed.

$json = [Console]::In.ReadToEnd()

try {
    $data = $json | ConvertFrom-Json
    $command = $data.tool_input.command
    if (-not $command) { $command = $data.tool_input.cmd }
    if ($command -match '^\s*git\s+(add|commit|push|pull|merge|rebase|checkout|switch|reset|clean|stash)\b') {
        [Console]::Error.WriteLine("Git write operation blocked by governance policy. Ask the user to perform this operation manually in the terminal.")
        exit 2
    }
} catch {
    # If JSON parsing fails, allow the operation.
}

exit 0
