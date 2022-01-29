$ErrorActionPreference = "Stop"

Function check-result {
    if ($LastExitCode -ne 0) { Invoke-BuildError "ERROR: Exiting with error code $LastExitCode"	}
    return $true
}

Function Invoke-BuildError {
Param(
	[parameter(Mandatory=$true)][string] $text
)
	if ($env:TEAMCITY_VERSION) {
        Write-Error "##teamcity[message text='$text']" # so number of failed tests shows in builds list instead of this text
		Write-Error "##teamcity[buildStatus status='ERROR']"
		[System.Environment]::Exit(1) 
	} else {
        Write-Error $text 
        exit
	}
}

Function Invoke-Exe {
    Param(
        [parameter(Mandatory = $true)][string] $cmd,
        [parameter(Mandatory = $true)][string] $args
	
    )
    Write-Output "Executing: `"$cmd`" --% $args"
    & "$cmd" "--%" "$args";
    $result = check-result
    if (!$result) {
        throw "ERROR executing EXE"
        exit 1
    }
}

Invoke-Exe -cmd git -args "checkout master"
Invoke-Exe -cmd git -args "pull"
Invoke-Exe -cmd git -args "checkout develop"
Invoke-Exe -cmd git -args "pull"
Invoke-Exe -cmd git -args "merge master"
Invoke-Exe -cmd git -args "push"

$a = Get-Content './src/version.json' -raw | ConvertFrom-Json
$branch = "release/$($a.version)"
echo $branch

$exists = (git ls-remote origin $branch)
if ($exists.Length -eq 0) {
	git checkout -b $branch
	./generate-changelog.ps1
	git add CHANGELOG.md
	git commit -m "generate changelog"
	git push
	git push --set-upstream origin $branch
	git checkout develop
	git merge $branch
	./update-version.ps1
	git commit -m "update version" .\src\version.json
	git push
} else {
	echo "release branch already exists"
}
