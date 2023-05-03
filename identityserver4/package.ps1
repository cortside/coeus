[cmdletBinding()]
Param(
[string]$version = "0.1",
[string]$suffix = "local",
[string]$msbuildconfig = "debug",
[ValidateSet("true", "false")]
[string]$traefikconfig = "false",
[string]$octopusServer = "http://octopus.Acmeusa.com",
[string]$octopusApiKey = "",
[string]$octopusUploadEndpoint = "$octopusServer/api/packages/raw"
)

. ".\build-common.ps1"

$build = gc $PSScriptRoot\src\build.json -raw | ConvertFrom-json

$filter = "*.nuspec"
Get-ChildItem -include $filter -Recurse | ? {$_.Fullname -notmatch 'packages'} | Resolve-Path -Relative |
ForEach-Object{
	Write-Host "Found: $_"
	$filepath = Get-ChildItem $_
	
	$nuspecpath = $_
	$nuspecfile = $filepath.Name
	$projname = $filepath.BaseName
	$projpath = $nuspecpath -replace ".nuspec", ".csproj"
	$tomlpath = "$PSScriptRoot\$($projname).toml"
	
	write-verbose "projname = $projname"
	write-verbose "projpath = $projpath"
	write-verbose "nuspecfile = $nuspecfile"
	write-verbose "version = $version"
	write-verbose "suffix = $suffix"
	write-verbose "nuspecpath = $nuspecpath"
	write-verbose "msbuildconfig = $msbuildconfig"

	$args = "pack $PSScriptRoot\src\$projpath /p:NuspecFile=.\$nuspecfile --version-suffix $suffix /p:PackageVersion=$version --output $PSScriptRoot\artifacts --no-build --no-restore /p:NuspecProperties=\`"Configuration=$msbuildconfig;version=$version\`""

	write-verbose "args = $args"

	Invoke-Exe -cmd dotnet -args $args
}

$filter = "*.toml"
Get-ChildItem -include $filter -Recurse | ? {$_.Fullname -notmatch 'packages'} | Resolve-Path -Relative |
ForEach-Object{
	Write-Host "Found: $_"
	$filepath = Get-ChildItem $_
	
	$nuspecpath = $_
	$nuspecfile = $filepath.Name
	$projname = $filepath.BaseName
	$projpath = $nuspecpath -replace ".nuspec", ".csproj"
	$tomlpath = $_
	
	write-verbose "projname = $projname"
	write-verbose "projpath = $projpath"
	write-verbose "nuspecfile = $nuspecfile"
	write-verbose "version = $version"
	write-verbose "suffix = $suffix"
	write-verbose "nuspecpath = $nuspecpath"
	write-verbose "msbuildconfig = $msbuildconfig"

	if ($traefikconfig -eq "true") {
		Compress-Archive -Path $tomlpath -CompressionLevel Fastest -DestinationPath $PSScriptRoot\$projname.$version.zip

		$url = "$($octopusUploadEndpoint)?apiKey=$($octopusApiKey)"
		$filePath = "$PSScriptRoot\$($projname).$($version).zip" 
		Write-Host "Uploading $filePath"
		$wc = new-object System.Net.WebClient
		$wc.UploadFile($url, $filePath)
		
		Remove-Item $filePath -force
	}
}
