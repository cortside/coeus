[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][ValidateSet("true", "false")][string]$local = "true",
	[Parameter(Mandatory = $false)][string]$username,
	[Parameter(Mandatory = $false)][string]$password,
	[Parameter(Mandatory = $false)][switch]$pushImage,
	[Parameter(Mandatory = $false)][string]$BuildCommitHash = $env:CommitHash,
	[Parameter(Mandatory = $false)][string]$RepositorySlug = $env:RepositorySlug,
	[Parameter(Mandatory = $false)][string]$acr = "cortside",
	[Parameter(Mandatory = $false)][string]$buildCounter = "0"
)

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';

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
	#$buildobject | ConvertTo-Json -Depth 5 | Out-File $buildjsonpath -force

	return $buildobject
}

if ($systemprune.IsPresent) {	
	Invoke-Exe -cmd docker -args "system prune --force"
}

$BuildNumber = (New-BuildJson -versionJsonPath $PSScriptRoot\repository.json -BuildJsonPath $PSScriptRoot\src\$publishableProject\build.json -buildCounter $buildCounter).build.version

#Run Build for all Dockerfiles in /Docker path
$dockerFiles = Get-ChildItem -Path . -Filter "Dockerfile.*" -Recurse
foreach ($dockerfile in $dockerFiles) {
	$dockerFileName = $dockerfile.name 
	Write-Output "Building $dockerFileName"

	$image = Split-Path -Path $dockerfile.DirectoryName -Leaf -Resolve
	$imageversion = $dockerfile.Name.replace('Dockerfile.','')
	$dockerbuildargs = "build --rm -t ${acr}/${image}:${BuildNumber}-${imageversion} -t ${acr}/${image}:${imageversion} -f $($dockerfile.FullName) $($dockerfile.DirectoryName)"
	Invoke-Exe -cmd docker -args $dockerbuildargs

	#Docker push images to repo
	if ($pushImage.IsPresent) {	
		write-output "pushing ${acr}/${image}:${imageversion}"
		$dockerpushargs = "push `"${acr}/${image}:${imageversion}`""
		Invoke-Exe -cmd docker -args $dockerpushargs

		write-output "pushing ${acr}/${image}:${BuildNumber}-${imageversion}"
		$dockerpushargs = "push `"${acr}/${image}:${BuildNumber}-${imageversion}`""
		Invoke-Exe -cmd docker -args $dockerpushargs
	} else {
		write-output "This is a local build and will not need to push."
	}

	#List images for the current tag
	Write-Output "Docker Just successfully built - ${acr}/${image}:${imageversion}"
}