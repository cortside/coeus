[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][ValidateSet("true", "false")][string]$local = "true",
	[Parameter(Mandatory = $false)][string]$username,
	[Parameter(Mandatory = $false)][string]$password,
	[Parameter(Mandatory = $false)][switch]$pushImage,
	[Parameter(Mandatory = $false)][string]$BuildCommitHash = $env:CommitHash,
	[Parameter(Mandatory = $false)][string]$RepositorySlug = $env:RepositorySlug,
	[Parameter(Mandatory = $false)][string]$acr = "cortside"
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

#Run Build for all Dockerfiles in /Docker path
$dockerFiles = Get-ChildItem -Path . -Filter "Dockerfile.*" -Recurse
foreach ($dockerfile in $dockerFiles) {
	$dockerFileName = $dockerfile.name 
	Write-Output "Building $dockerFileName"

	$image = Split-Path -Path $dockerfile.DirectoryName -Leaf -Resolve
	$imageversion = $dockerfile.Name.replace('Dockerfile.','')
	$dockerbuildargs = "build --rm -t ${acr}/${image}:${imageversion} -f $($dockerfile.FullName) ."
	Invoke-Exe -cmd docker -args $dockerbuildargs

	#Docker push images to repo
	if ($pushImage.IsPresent) {	
		write-output "pushing ${acr}/${image}:${imageversion}"
		$dockerpushargs = "push `"${acr}/${image}:${imageversion}`""
		Invoke-Exe -cmd docker -args $dockerpushargs
	} else {
		write-output "This is a local build and will not need to push."
	}

	#List images for the current tag
	Write-Output "Docker Just successfully built - ${acr}/${image}:${imageversion}"
}