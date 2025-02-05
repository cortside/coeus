<#
    .SYNOPSIS
        Controller script for building docker image(s)
    .DESCRIPTION
        This script is responsible for creating the docker image(s) and optionally
        pushing them to the registry when done.  This script should be build system agnostic and be
		able to be run by engineers locally for testing.  Should not require knowledge of secrets, 
		passwords or tokens.
#>

[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][string]$branch = "",
	[Parameter(Mandatory = $false)][string]$target = "develop",
	[Parameter(Mandatory = $false)][string]$commit = "",
	[Parameter(Mandatory = $false)][string]$commitdate = "",
	[Parameter(Mandatory = $false)][string]$pullRequestId = "",
	[Parameter(Mandatory = $false)][string]$buildCounter = "0",
	[Parameter(Mandatory = $false)][switch]$pushImage
)

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';

# common repository functions
if (!(Test-Path .\repository.psm1)) {
	Write-Error "This script must be run from the root of the repository, i.e. ./deploy/$($MyInvocation.MyCommand)"
	exit 1
}
Import-Module .\repository.psm1 -Force

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
	Write-Host "Executing: `"$cmd`" $args"
	Invoke-Expression "& `"$cmd`" $args"
	$result = Get-Result
}

Function Get-BranchTag {
	[OutputType([string])]
	param(
		[Parameter(Mandatory = $true)]
		[string]$branchName
	)

	$tagPart = $branchName 
	if ($branchName -like "master") { 
		$tagPart = "" 
	} elseif ($branchName -like "release/*") { 
		$tagPart = "release" 
	} elseif ($branchName -like "bugfix/*" -or $branchName -like "hotfix/*" -or $branchName -like "feature/*") {
		# extract jira key, i.e. CFG-123
		$tagPart = ($branchName | Select-String -Pattern '((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)' | % matches).value
		$tagPart = $tagPart -replace '-',''
	}
	
	#if ($tagPart -ne "") {
	#	$tagPart = "-$tagPart"
	#}
	
	$tagPart
}

## set values for those that are not set
$local = "false"
if ($buildCounter -eq "0") {
	$local = "true"
}
if ($branch -eq "") {
	$branch = (git rev-parse --abbrev-ref HEAD)
}

if (Test-Path env:CommitHash) {
	$commit = $env:CommitHash
}
if ($commit -eq "") {
	$commit = (git rev-parse HEAD)
}
if ($commitdate -eq "") {
	$commitdate = (git log -1 --format="%aI")
}

$config = Get-RepositoryConfiguration
$version = "$($config.version).$buildCounter"
$branchTag = Get-BranchTag($branch)
$imageversion = "$($version)"
if ($branchTag -ne "") {
	$imageversion += "-$($branchTag)"
}

$dockerpath = "Dockerfile.*"
$dockercontext = "."

## export key values to json file for other processes to read from
$export = [ordered]@{}
$export.service = $($config.service)
$export.build = [ordered]@{}
$export.build.timestamp = Get-Date -Format "yyyy-MM-ddTHH:mmK"
$export.build.version = $version
$export.build.tag = $imageversion
$export.build.suffix = $branchTag
$export.version= $version
$export.branch= $branch
$export.branchTag= $branchTag
$export.commit= $commit
$export.commitdate= $commitdate
$export.dockerpath= $dockerpath
$export.dockercontext= $dockercontext
$export.buildconfiguration= $($config.build.configuration)
$export.nugetfeed=$($config.build.nugetfeed)
$export.buildimage=$($config.docker.buildimage)
$export.runtimeimage=$($config.docker.runtimeimage)
$export.image=$($config.docker.image)
$export.imageversion=$imageversion
$export.buildCounter = $buildCounter
$export.local = $local
$export.pushImage = $pushImage.IsPresent
$export.timezone = $($config.docker.timezone)
$export.images = @()

$export | ConvertTo-Json -Depth 5

#Run Build for all Dockerfiles in /Docker path
$dockerFiles = Get-ChildItem -Path $dockercontext -Filter $dockerpath -Recurse
foreach ($dockerfile in $dockerFiles) {
	$image = [ordered]@{}
    $image.dockerfile = $dockerfile | Resolve-Path -Relative
	$image.dockerFilename = $dockerfile.name
	$image.hostOS = $image.dockerFileName.split(".").split()[-1]
	$image.image = $config.docker.image
	
	$image.imageversion = "$($config.docker.image):${imageversion}"
	$image.branchTag = "$($config.docker.image)"
	if ($branchTag -ne "") {
		$image.branchTag += ":${branchTag}"
	}
	
	if ($image.hostOS -ne "") {
		if ($image.branchTag -like "*:*") {
			$image.branchTag += "-$($image.hostOS)"
		} else {
			$image.branchTag += ":$($image.hostOS)"
		}
		$image.imageversion += "-$($image.hostOS)"
	}

	[string[]]$tags = @()
	$tags += $image.imageversion
	$tags += $image.branchTag

	if ($branch -like "release/*") {
		$t = "$($config.docker.image):$($config.version)-release"
		if ($image.hostOS -ne "") {
			$t += "-$($image.hostOS)"
		}
	
		$tags += $t
	}
	
	if ($branch -eq "master") {
		$tags += "$($config.docker.image):latest"
		$t = "$($config.docker.image):$($config.version)"
		if ($image.hostOS -ne "") {
			$t += "-$($image.hostOS)"
		}
		
		$tags += $t
	}
	
	# sonar
	$analysisArgs = "$($config.sonar.propertyPrefix)sonar.scm.disabled=true";
	#if (-not (Test-Path env:APPVEYOR_PULL_REQUEST_NUMBER)) {
	if ($pullRequestId -eq "") {
		#$branch = $Env:APPVEYOR_REPO_BRANCH;
		$analysisArgs += " $($config.sonar.propertyPrefix)sonar.branch.name=$branch";
		if ($branch -ne "master") {
			#$target = "develop";
			#if ($branch -eq "develop" -or $branch -like "release/*" -or $branch -like "hotfix/*") {
			#    $target = "master";
			#}
			$analysisArgs += " $($config.sonar.propertyPrefix)sonar.newCode.referenceBranch=$target";
		}
	} else {
		#$branch = $Env:APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH;
		#$target = $Env:APPVEYOR_REPO_BRANCH;
		#$commit = $Env:APPVEYOR_PULL_REQUEST_HEAD_COMMIT;
		#$pullRequestId = $Env:APPVEYOR_PULL_REQUEST_NUMBER;
		$analysisArgs += " $($config.sonar.propertyPrefix)sonar.scm.revision=$commit $($config.sonar.propertyPrefix)sonar.pullrequest.key=$pullRequestId $($config.sonar.propertyPrefix)sonar.pullrequest.base=$target $($config.sonar.propertyPrefix)sonar.pullrequest.branch=$branch";
	}

echo $analysisArgs

	#$sonarArgs = "--build-arg `"analysisArgs=$analysisArgs`" --build-arg `"sonarhost=$($config.sonar.host)`" --build-arg `"sonartoken=$($config.sonar.token)`" --build-arg `"sonarkey=$($config.sonar.key)`""
	#}

	$image.tags = @()
	$image.tags += $tags
	$export.images += $image

	$image | ConvertTo-Json -Depth 5

	Write-Output "Building $($image.dockerfile)"
	
	$dockerbuildargs = @()
	$dockerbuildargs += "build"
	$dockerbuildargs += "--rm"

	$dockerbuildargs += "--no-cache"
	$dockerbuildargs += "--progress=plain"
	
	$dockerbuildargs += "--build-arg `"analysisArgs=$analysisArgs`""
	$dockerbuildargs += "--build-arg `"sonarhost=$($config.sonar.host)`""
	$dockerbuildargs += "--build-arg `"sonartoken=$($config.sonar.token)`""
	$dockerbuildargs += "--build-arg `"sonarkey=$($config.sonar.key)`""
	$dockerbuildargs += "--build-arg `"organization=$($config.sonar.organization)`""

	$dockerbuildargs += "--build-arg `"buildimage=$($config.docker.buildimage)`""
	$dockerbuildargs += "--build-arg `"runtimeimage=$($config.docker.runtimeimage)`""
	$dockerbuildargs += "--build-arg `"service_name=$($config.service)`""
	$dockerbuildargs += "--build-arg `"service_executable=$($config.docker.executable)`""
	$dockerbuildargs += "--build-arg `"publishableProject=$($config.build.publishableProject)`""
	$dockerbuildargs += "--build-arg `"buildconfiguration=$($config.build.configuration)`""
	$dockerbuildargs += "--build-arg `"nugetfeed=$($config.build.nugetfeed)`""
	$dockerbuildargs += "--build-arg `"branch=$branch`""
	$dockerbuildargs += "--build-arg `"commit=$commit`""
	$dockerbuildargs += "--build-arg `"commitdate=$commitdate`""
	$dockerbuildargs += "--build-arg `"branchTag=$branchTag`""
	$dockerbuildargs += "--build-arg `"imageversion=$imageversion`""
	$dockerbuildargs += "--build-arg `"version=$version`""
	$dockerbuildargs += "--build-arg `"timezone=$($config.docker.timezone)`""

	## add any build args from docker.env
	if ((Test-Path -path "docker.env")) {
		$dockerbuildargs += "--build-arg $((Get-Content -Path "docker.env") -join ' --build-arg ')"
	}
	
	foreach ($tag in $image.tags) {
		$dockerbuildargs += "-t $($tag)"
	}
	$dockerbuildargs += "-f $($image.dockerfile) $dockercontext"

	$dockerbuildargs = $dockerbuildargs -join " "
	
	Invoke-Exe -cmd docker -args "pull $($config.docker.buildimage)"
	Invoke-Exe -cmd docker -args "pull $($config.docker.runtimeimage)"
	Invoke-Exe -cmd docker -args $dockerbuildargs

	#Docker push images to repo
	if ($pushImage.IsPresent) {	
		write-output "pushing $($image.imageversion)"
		$dockerpushargs = "push --all-tags $($image.image)"
		Invoke-Exe -cmd docker -args $dockerpushargs
	} else {
		write-output "This is a local build and will not need to push."
	}

	#List images for the current tag
    Write-Output "Image successfully built - $($image.imageversion)"
}

$export | ConvertTo-Json -Depth 5 | Out-File -FilePath buildconfig.json
cat buildconfig.json
