[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][string]$branch,
	[Parameter(Mandatory = $false)][string]$dockerpath = "Dockerfile.*",
	[Parameter(Mandatory = $true)][string]$dockercontext,
	[Parameter(Mandatory = $false)][string]$buildconfiguration = "Debug",
	[Parameter(Mandatory = $false)][ValidateSet("true","false")][string]$local = "true",
	[Parameter(Mandatory = $false)][string]$OctopusEndpoint,
	[Parameter(Mandatory = $false)][string]$OctopusApiKey,
	[Parameter(Mandatory = $false)][string]$nugetfeed = "http://oa-utility.Acme.local:81/nuget/develop/",
	[Parameter(Mandatory = $false)][string]$OctopusVersion, 
	[Parameter(Mandatory = $false)][string]$username,
	[Parameter(Mandatory = $false)][string]$password
)

#Load environment variables from PS folder
. $PSScriptRoot\version.ps1

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
    [parameter(Mandatory=$true)][string] $cmd,
    [parameter(Mandatory=$true)][string] $args

)
	Write-Host "Executing: `"$cmd`" --% $args"
	Invoke-Expression "& `"$cmd`" --% $args"
	$result = Get-Result
}

Write-Verbose $version
Write-Verbose "branch: $branch"
Write-Verbose "dockerpath: $dockerpath"
Write-Verbose "dockercontext: $dockercontext"
Write-Verbose "buildconfiguration: $buildconfiguration"
Write-Verbose "local: $local"
Write-Verbose "OctopusEndpoint: $OctopusEndpoint"
Write-Verbose "nugetfeed: $nugetfeed"
Write-Verbose "OctopusVersion: $OctopusVersion"
Write-Verbose "ngversion=$ngversion"
Write-Verbose "npmversion=$npmversion"
Write-Verbose "dotnetsdk=$dotnetsdk"
Write-Verbose "dotnetframework=$dotnetframework"
Write-Verbose "nginxversion=$nginxversion"
Write-Verbose "image:$image"
Write-Verbose "containerregistry=$acr"

if ($OctopusVersion) {
	$version = $OctopusVersion
	$v = $OctopusVersion
	Write-Output "Using version of $version"
} 
else {
	$v = $version
	Write-Output "Using Default version of $version"
}

if ($local -eq "true" ) {
	$env:SonarToken = $localsonartoken
	$env:SonarHost = $localsonarhost
	$branch = Invoke-Exe -cmd git -args "rev-parse --abbrev-ref HEAD"
} 
else {
	Write-Output "====`n Docker Login`n ===="
	docker login $acr -u $username -p $password
}

#Run Build for all Dockerfiles in /Docker path
$dockerFiles = Get-ChildItem -Path $dockercontext -Filter $dockerpath -Recurse
foreach ($dockerfile in $dockerFiles) {
	$dockerFileName = $dockerfile.name 
	$HostOS = $dockerFileName.split(".").split()[-1]
	Write-Output "Building $dockerFileName"
	Write-Output "HostOS is $HostOS"
	$imageversion = "$v-$HostOS"

	$dockerbuildargs = "build --rm --add-host=oa-utility.Acme.local:10.10.10.207 --build-arg `"buildconfiguration=$buildconfiguration`" --build-arg `"nugetfeed=$nugetfeed`" --build-arg `"dotnetsdk=$dotnetsdk`" --build-arg `"dotnetframework=$dotnetframework`" --build-arg `"branch=$branch`" --build-arg `"imageversion=$imageversion`" --build-arg `"sonarhost=$($env:SonarHost)`" --build-arg `"sonarkey=$($sonarkey)`" --build-arg `"sonartoken=$($env:SonarToken)`" --build-arg `"projectname=$($projectname)`" --build-arg `"sonarscannerversion=$($sonarscannerversion)`" -t ${acr}/${image}:${imageversion} -f deploy/docker/$dockerFileName $dockercontext"

	#Docker build and tag
        Write-Output $dockerbuildargs
	Write-Output "Dockerfile used: $dockerFileName"
	Invoke-Exe -cmd docker -args $dockerbuildargs

	#Docker push images to repo
	if ($local -eq "true"){
		write-output "This is a local build and will not need to push."
	}else{
		$imageversion = "$v-$HostOS"
		write-output "pushing ${acr}/${image}:${imageversion}"
		$dockerpushargs = "push ${acr}/${image}:${imageversion}"
		Invoke-Exe -cmd docker -args $dockerpushargs

		write-output "cleaning up ${acr}/${image}:${imageversion}"
		$dockerrmiargs = "rmi ${acr}/${image}:${imageversion}"
		Invoke-Exe -cmd docker -args $dockerrmiargs
	}

	#List images for the current tag
	Write-Output "Docker Just successfully built - ${acr}/${image}:${imageversion}"
	Write-Output "`tPlease run with any additional flags to test locally:`n`n docker run -d ${acr}/${image}:${imageversion}"
	Write-Output "`t --------------- `t Docker run reference if needed:`n https://docs.docker.com/engine/reference/run/`n"
}
$configDir = (Get-ChildItem -Filter "*.WebApi" -Recurse).FullName
$configFile = (Get-ChildItem -Path $configDir -Filter "config.json").FullName
$appconfig = (Get-ChildItem -Path $configDir -Filter "appsettings*.json").FullName
$configs = @()
$configs = $configs += $appconfig
$configs = $configs += $configFile


Compress-Archive -Force -Path $PSScriptRoot\version.ps1 -CompressionLevel Fastest -DestinationPath $PSScriptRoot\$image.$v.zip
Compress-Archive -Path $PSScriptRoot\..\kubernetes\* -Update -DestinationPath $PSScriptRoot\$image.$v.zip
foreach($config in $configs){Compress-Archive -Path $config -Update -DestinationPath $PSScriptRoot\$image.$v.zip}
# add database files for migrations to package
Compress-Archive -Path $PSScriptRoot\..\..\update-database.ps1 -Update -DestinationPath $PSScriptRoot\$image.$v.zip
Copy-Item -Path $PSScriptRoot\..\..\src\sql -Destination $PSScriptRoot\temp\src\sql -Recurse # to preserve structure for the script
Compress-Archive -Path $PSScriptRoot\temp\src -Update -DestinationPath $PSScriptRoot\$image.$v.zip
Remove-Item -Path $PSScriptRoot\temp -Recurse -Force
Write-Output "`n=== Created zip package $image.$v.zip in $PSScriptRoot ===`n"

Write-Output "End of Build"
