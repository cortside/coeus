[cmdletbinding()]
param(
	[string]$branch = "local",
	[string]$buildCounter = "0",
	[string]$msbuildconfig = "Debug"
)

. ".\build-common.ps1"

# generate build.json
$BuildNumber = (New-BuildJson -versionJsonPath $PSScriptRoot\src\version.json -BuildJsonPath $PSScriptRoot\src\build.json -buildCounter $buildCounter).build.version
Write-Host "##teamcity[buildNumber '$BuildNumber']"
$build = Set-DockerTag -branch $branch -buildNumber $BuildNumber -BuildJsonPath $PSScriptRoot\src\build.json
$dockertag = $build.build.tag
$suffix = $build.build.suffix
$OctopusChannel = $build.build.channel
Write-output = "
--------
Dockertag for this build: $dockertag
++++++++
Suffix for this build: $suffix
++++++++
OctopusChannel for this build: $OctopusChannel
--------
"

Write-Host "##teamcity[setParameter name='env.dockertag' value='$dockertag']"
Write-Host "##teamcity[setParameter name='env.OctopusChannel' value='$OctopusChannel']"
Write-Host "##teamcity[setParameter name='env.suffix' value='$suffix']"
Write-Host "##teamcity[setParameter name='env.MsBuildConfig' value='$msbuildconfig']"

if ($suffix){
	$OctopusVersion = "$BuildNumber-$suffix"
	Write-Host "##teamcity[setParameter name='env.OctopusVersion' value='$OctopusVersion']"	
}elseif(!$suffix){
	$OctopusVersion = "$BuildNumber"
	Write-Host "##teamcity[setParameter name='env.OctopusVersion' value='$OctopusVersion']"
}

# build
#Find all sln files and build
$filter = "*.sln"
Get-ChildItem -include $filter -Recurse | Resolve-Path -Relative |
ForEach-Object{
	Write-Host "Found: $_"

	Invoke-Exe -cmd dotnet -args "clean $_"
	$args = "restore $_ --packages $PSScriptRoot\src\packages"
	Invoke-Exe -cmd dotnet -args $args
	$args = "build $_ --no-restore --configuration $msbuildconfig /p:Version=$BuildNumber"
	Invoke-Exe -cmd dotnet -args $args
}

# publish all publishable projects
$filter = "*.csproj"
Get-ChildItem -include $filter -Recurse | Resolve-Path -Relative |
ForEach-Object{
	Write-Host "Found: $_"
	$needle = "<OutputType>Exe</OutputType>"

	$file = Get-Content $_
	$hasNeedle = $file | %{$_ -match $needle}
	If($hasNeedle -contains $true) {
		# copy generated build.json to needed applications
		$f = gci $_
		Write-Host "copy to: $($f.Directory)"
		cp .\src\build.json $f.Directory -force

		$args = "publish $_ --no-restore /p:Version=$BuildNumber -o $PSScriptRoot\publish"
		Invoke-Exe -cmd dotnet -args $args
	}
}

