[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][string]$branch = "local",
	[Parameter(Mandatory = $false)][string]$buildCounter = "0",
	[Parameter(Mandatory = $false)][ValidateSet("true", "false")][string]$local = "true",
	[Parameter(Mandatory = $false)][switch]$systemprune,
	[Parameter(Mandatory = $false)][switch]$pushImage
)

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';

# common repository functions
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

	if ($branchName -eq "master") { 
		$tagPart = "master" 
	} elseif ($branchName -eq "develop") { 
		$tagPart = "develop" 
	} elseif ($branchName -like "release/*") { 
		$tagPart = "release" 
	} elseif ($branchName -like "bugfix/*" -or $branchName -like "hotfix/*" -or $branchName -like "feature/*") {
		# extract jira key, i.e. CFG-123
		$tagPart = ($branchName | Select-String -Pattern '((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)' | % matches).value
	} else { 
		$tagPart = $branchName 
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

$config = Get-RepositoryConfiguration
$BuildNumber = (New-BuildJson -versionJsonPath $PSScriptRoot\repository.json -BuildJsonPath $PSScriptRoot\src\$($config.build.publishableProject)\build.json -buildCounter $buildCounter).build.version

$dockerpath = "Dockerfile.*"
$dockercontext = "."

Write-Output $version
Write-Output "buildNumber: $buildNumber"
Write-Output "branch: $branch"
Write-Output "dockerpath: $dockerpath"
Write-Output "dockercontext: $dockercontext"
Write-Output "buildconfiguration: $buildconfiguration"
Write-Output "local: $local"
Write-Output "nugetfeed: $($config.nuget.feed)"
Write-Output "buildimage=$($config.docker.buildimage)"
Write-Output "runtimeimage=$($config.docker.runtimeimage)"
Write-Output "image:$($config.docker.image)"

#Run Build for all Dockerfiles in /Docker path
$dockerFiles = Get-ChildItem -Path $dockercontext -Filter $dockerpath -Recurse
foreach ($dockerfile in $dockerFiles) {
	#Docker build and tag
	$branchTag = Get-BranchTag -branchName $branch

	$dockerFileName = $dockerfile.name 
	$HostOS = $dockerFileName.split(".").split()[-1]
	Write-Output "Building $dockerFileName"
	$imageversion = "$buildNumber-$branchTag-$HostOS"

	$sonarArgs = "--build-arg `"analysisArgs=$analysisArgs`" --build-arg `"sonarhost=$($config.sonar.host)`" --build-arg `"sonartoken=$($config.sonar.token)`" --build-arg `"sonarkey=$($config.sonar.key)`""

	$dockerbuildargs = @()
	$dockerbuildargs +="build"
	$dockerbuildargs +="--rm"
	$dockerbuildargs +="--progress=plain"
	$dockerbuildargs +="--add-host=proget.local:10.10.10.10"
	$dockerbuildargs +="--build-arg `"service_name=$($config.service)`""
	$dockerbuildargs +="--build-arg `"service_executable=$($config.docker.executable)`""
	$dockerbuildargs +="--build-arg `"artifactDirectory=$($config.docker.artifactDirectory)`""
	$dockerbuildargs +="--build-arg `"organization=$($config.sonar.organization)`""
	$dockerbuildargs +="--build-arg `"publishableProject=$($config.build.publishableProject)`""
	$dockerbuildargs +="--build-arg `"buildconfiguration=$($config.build.configuration)`""
	$dockerbuildargs +="--build-arg `"nugetfeed=$($config.build.nugetfeed)`""
	$dockerbuildargs +="--build-arg `"buildimage=$($config.docker.buildimage)`""
	$dockerbuildargs +="--build-arg `"runtimeimage=$($config.docker.runtimeimage)`""
	$dockerbuildargs +="--build-arg `"branch=$branch`""
	$dockerbuildargs +="--build-arg `"imageversion=$imageversion`""
	$dockerbuildargs +="--build-arg `"projectname=$($config.repository.name)`" $sonarArgs -t $($config.docker.image):${branchTag} -t $($config.docker.image):${imageversion} -f deploy/docker/$dockerFileName $dockercontext"

	$dockerbuildargs = $dockerbuildargs -join " "
	
	Invoke-Exe -cmd docker -args $dockerbuildargs

	#Docker push images to repo
	if ($pushImage.IsPresent) {	
		write-output "pushing $($config.docker.image):${imageversion}"
		$dockerpushargs = "push --all-tags $($config.docker.image)"
		Invoke-Exe -cmd docker -args $dockerpushargs
	} else {
		write-output "This is a local build and will not need to push."
	}

	#List images for the current tag
	Write-Output "Docker Just successfully built - ${image}:${imageversion}"
	Write-Output "`tPlease run with any additional flags to test locally:`n`n docker run -d ${image}:${imageversion}"
	Write-Output "`t --------------- `t Docker run reference if needed:`n https://docs.docker.com/engine/reference/run/`n"
}