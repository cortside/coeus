# Getting version number from project
$a = Get-Content './src/version.json' -raw | ConvertFrom-Json
$currentVersion = $a.version

# TODO: find current version in file if it exists and truncate to the end

$commits = (git --no-pager log --pretty=format:'| %h | <span style="white-space:nowrap;">%ad</span> | <span style="white-space:nowrap;">%aN</span> | %d %s' --date=short master.. | tac)

"" | Out-File CHANGELOG.md -Encoding utf8 -Append
"# Release $currentVersion" | Out-File CHANGELOG.md -Encoding utf8 -Append
"" | Out-File CHANGELOG.md -Encoding utf8 -Append
"|Commit|Date|Author|Message|" | Out-File CHANGELOG.md -Encoding utf8 -Append
"|---|---|---|---|" | Out-File CHANGELOG.md -Encoding utf8 -Append
$commits | Out-File CHANGELOG.md -Encoding utf8 -Append
"****" | Out-File CHANGELOG.md -Encoding utf8 -Append
"" | Out-File CHANGELOG.md -Encoding utf8 -Append
