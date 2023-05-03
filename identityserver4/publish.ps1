[cmdletBinding()]
Param(
	[string]$dockerlogin = "Acme.azurecr.io"
)

$build = gc $PSScriptRoot\src\build.json -raw | ConvertFrom-json
$tag = $build.build.tag

Write-Host "DOCKER_HOST: $env:DOCKER_HOST"
Write-Host "dockerlogin: $dockerlogin"

# publish all publishable projects
$filter = "*.dockerfile"
Get-ChildItem -include $filter -Recurse | Resolve-Path -Relative |
ForEach-Object{
	Write-Host "Found: $_"
	$filepath = Get-ChildItem $_
	$image = "$dockerlogin/$($filepath.BaseName)"
	Write-Host "Building ${image}:${tag}"

	docker build --no-cache --pull -t ${image}:${tag} -f $_ .
	docker images | select-string $image

	if (!$tag.EndsWith("-local")) {
		docker push ${image}:${tag}
	}
}
