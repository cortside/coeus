[cmdletBinding()]
Param(
    [Parameter(Mandatory=$false)][switch]$SkipBuild
)

Write-Output @("
##########
# 1. Build the solution (can be overridden with -SkipBuild parameter)
# 2. Build a fresh database so that all migrations are applied
# 3. Generate new trigger sql scripts 
# 4. Convert encoding on updated scripts to UTF-8
##########
")
start-sleep -Seconds 3

$serverInstance = "localhost"

if ($PSScriptRoot.Contains(' ')) { 
    throw "Your working directory has a space in the path, which is not supported.  Wise up and move to C:\work\Acme.users! And have a wonderful day, or evening!"
    exit
}

try {
	Import-Module SqlServer -ErrorAction Stop
}
catch {
    Write-Output "Installing SqlServer module for powershell"
	Install-PackageProvider -Name NuGet -Force
	Install-Module -Name SqlServer -AllowClobber -Force
	Import-Module SqlServer
}

try {
    Write-Output "Verifying SqlServer accessible at $serverInstance"
    invoke-sqlcmd -ServerInstance $serverInstance -Query "select 'invoke-sqlcmd successful' AS SqlServerStatus" -QueryTimeout 5 -ConnectionTimeout 5 -ErrorAction Stop
} catch {
    throw "Problem connecting to SqlServer at $serverInstance. Please confirm up and running and try again."
}

#if ($SkipBuild.IsPresent) {
#    Write-Warning "Skipped build of solution"
#} else {
#    Write-Output "build solution"
#    & (Get-Item "$PSScriptRoot\build.ps1").FullName
#}

Write-Output "update trigger scripts"
$generateTriggersPath = (Get-ChildItem -Path $PSScriptRoot -Recurse -Filter "GenerateTriggers.sql").FullName
Write-Output $generateTriggersPath
$outputPathVariable = "sqlcmdPath=$PSScriptRoot\src\sql\trigger"
invoke-sqlcmd -Variable $outputPathVariable -ServerInstance $serverInstance -inputFile $generateTriggersPath -Database "IdentityServer" -QueryTimeout 240 -ConnectionTimeout 240 -ErrorAction Stop

Write-Output "convert encoding"
& "$PSScriptRoot\Convert-Encoding.ps1" -filePaths ((gci -Path $PSScriptRoot -Filter "*.trigger.sql" -Recurse) | % { $_.FullName })

git status

Write-Output @("
##########
# Complete. Updated files for commit listed above
##########
")
