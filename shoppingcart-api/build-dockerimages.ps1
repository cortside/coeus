[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][string]$branch = "local",
	[Parameter(Mandatory = $false)][string]$buildCounter = "0",
	[Parameter(Mandatory = $false)][string]$msbuildconfig = "Debug",
	[Parameter(Mandatory = $false)][string]$dockerpath = "Dockerfile.*",
	[Parameter(Mandatory = $false)][string]$dockercontext = ".",
	[Parameter(Mandatory = $false)][string]$buildconfiguration = "Debug",
	[Parameter(Mandatory = $false)][ValidateSet("true", "false")][string]$local = "true",
	[Parameter(Mandatory = $false)][string]$OctopusEndpoint,
	[Parameter(Mandatory = $false)][string]$OctopusApiKey,
	[Parameter(Mandatory = $false)][string]$nugetfeed = "https://api.nuget.org/v3/index.json",
	[Parameter(Mandatory = $false)][string]$OctopusVersion, 
	[Parameter(Mandatory = $false)][string]$username,
	[Parameter(Mandatory = $false)][string]$password,
	[Parameter(Mandatory = $false)][switch]$skipDbTest,
	[Parameter(Mandatory = $false)][switch]$systemprune,
	[Parameter(Mandatory = $false)][switch]$pushImage,
	[Parameter(Mandatory = $false)][string]$BuildCommitHash = $env:CommitHash,
	[Parameter(Mandatory = $false)][string]$RepositorySlug = $env:RepositorySlug,
	[Parameter(Mandatory = $false)][string]$sdkimage = "cortside/dotnet-sdk:6.0-alpine",
	[Parameter(Mandatory = $false)][string]$runtimeimage = "cortside/dotnet-runtime:6.0-alpine"
)

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';

#Load environment variables from PS folder
. $PSScriptRoot\deploy\ps\version.ps1

Function Get-Result {
	if ($LastExitCode -ne 0) {
		$text = "ERROR: Exiting with error code $LastExitCode"
		Write-Error "##teamcity[buildStatus status='$text']"
		if (-not ($local -eq 'true')) { [System.Environment]::Exit(1) }
	}
	return $true
}

Function Invoke-Exe {
	Param(
		[parameter(Mandatory = $true)][string] $cmd,
		[parameter(Mandatory = $true)][string] $args

	)
	if ($env:DOCKER_HOST -and $local -eq 'true') {
		Write-Host "Executing: `"$cmd`" $args"
		Invoke-Expression "& `"$cmd`" $args"
	} else {
		Write-Host "Executing: `"$cmd`" --% $args"
		Invoke-Expression "& `"$cmd`" --% $args"
	}
	$result = Get-Result
}

Function Get-BranchTag {
	[OutputType([string])]
	param(
		[Parameter(Mandatory = $true)]
		[string]$branchName
	)

	$tagPart = 
	if ($branchName -eq "master") { "master" }
	elseif ($branchName -eq "develop") { "develop" }
	elseif ($branchName -like "release/*") { "release" }
	elseif ($branchName -like "bugfix/*" -or $branchName -like "hotfix/*" -or $branchName -like "feature/*") {
		# extract jira key, i.e. CFG-123
		($branchName | Select-String -Pattern '((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)' | % matches).value
	} else { $branchName }
	
	if ($tagPart -eq "Invalid") {
		Write-Error "##teamcity[buildStatus status='No valid branch tag could be created. Check branch name.']"
	}

	$tagPart
}

Function New-BuildJson {
	Param (
		[Parameter(Mandatory = $true)][string]$versionjsonpath,
		[Parameter(Mandatory = $true)][string]$buildjsonpath,
		[Parameter(Mandatory = $true)][string]$buildCounter
	)

	$version = Get-Content $versionjsonpath -raw | ConvertFrom-Json
	$buildobject = New-Object -TypeName psobject
	$build = New-Object -TypeName psobject
	$builditems = [ordered] @{
		"version"   = ""
		"timestamp" = ""
		"tag"       = ""
		"suffix"    = ""
	}

	$NewBuildVersion = "$($version.version).$buildCounter"
	$buildTime = (Get-Date).ToUniversalTime().ToString("u")
	$builditems.version = $NewBuildVersion
	$builditems.timestamp = $buildTime
	$builditems.Keys | % { $build | Add-Member -MemberType NoteProperty -Name $_ -Value $builditems.$_ } > $null
	
	$buildobject | Add-Member -MemberType NoteProperty -Name Build -Value $build
	$buildobject | ConvertTo-Json -Depth 5 | Out-File $buildjsonpath -force

	return $buildobject
}

if ($systemprune.IsPresent) {	
	Invoke-Exe -cmd docker -args "system prune --force"
}

$BuildNumber = (New-BuildJson -versionJsonPath $PSScriptRoot\repository.json -BuildJsonPath $PSScriptRoot\src\$publishableProject\build.json -buildCounter $buildCounter).build.version

Write-Output $version
Write-Output "buildNumber: $buildNumber"
Write-Output "branch: $branch"
Write-Output "dockerpath: $dockerpath"
Write-Output "dockercontext: $dockercontext"
Write-Output "buildconfiguration: $buildconfiguration"
Write-Output "local: $local"
Write-Output "OctopusEndpoint: $OctopusEndpoint"
Write-Output "nugetfeed: $nugetfeed"
Write-Output "OctopusVersion: $OctopusVersion"
Write-Output "ngversion=$ngversion"
Write-Output "npmversion=$npmversion"
Write-Output "sdkimage=$sdkimage"
Write-Output "runtimeimage=$runtimeimage"
Write-Output "image:$image"

#Run Build for all Dockerfiles in /Docker path
$dockerFiles = Get-ChildItem -Path $dockercontext -Filter $dockerpath -Recurse
foreach ($dockerfile in $dockerFiles) {
	#Docker build and tag
	$branchTag = Get-BranchTag -branchName $branch

	$dockerFileName = $dockerfile.name 
	$HostOS = $dockerFileName.split(".").split()[-1]
	Write-Output "Building $dockerFileName"
	$imageversion = "$buildNumber-$branchTag-$HostOS"

	$sonarArgs = "--build-arg `"analysisArgs=$analysisArgs`" --build-arg `"sonarhost=$($SonarHost)`" --build-arg `"sonartoken=$($SonarToken)`" --build-arg `"sonarkey=$($sonarkey)`""

	$dockerbuildargs = "build --rm --log-level `"debug`" --add-host=proget.local:10.10.10.10 --build-arg `"organization=$organization`" --build-arg `"publishableProject=$publishableProject`" --build-arg `"buildconfiguration=$buildconfiguration`" --build-arg `"nugetfeed=$nugetfeed`" --build-arg `"sdkimage=$sdkimage`" --build-arg `"runtimeimage=$runtimeimage`" --build-arg `"branch=$branch`" --build-arg `"imageversion=$imageversion`" --build-arg `"projectname=$($projectname)`" $sonarArgs -t ${image}:${branchTag} -t ${image}:${imageversion} -f deploy/docker/$dockerFileName $dockercontext"
	Invoke-Exe -cmd docker -args $dockerbuildargs

	#Docker push images to repo
	if ($pushImage.IsPresent) {	
		write-output "pushing ${image}:${imageversion}"
		$dockerpushargs = "push --all-tags ${image}"
		Invoke-Exe -cmd docker -args $dockerpushargs
	} else {
		write-output "This is a local build and will not need to push."
	}

	#List images for the current tag
	Write-Output "Docker Just successfully built - ${image}:${imageversion}"
	Write-Output "`tPlease run with any additional flags to test locally:`n`n docker run -d ${image}:${imageversion}"
	Write-Output "`t --------------- `t Docker run reference if needed:`n https://docs.docker.com/engine/reference/run/`n"
}