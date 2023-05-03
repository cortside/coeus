[cmdletBinding()]
Param(
[string]$dockerlogin = "Acme.azurecr.io"
)
$build = gc $PSScriptRoot\src\build.json -raw | ConvertFrom-json


$app = "multidisbursement.api"
$image = "$dockerlogin/$app"
$tag = $build.build.tag

$env:DOCKER_HOST

docker stop $app
docker rm $app

docker pull ${image}:${tag}
docker run -d -p 24326:5000 --name $app ${image}:${tag}

docker exec $app ipconfig
docker exec $app ping 8.8.8.8

docker ps | select-string "$app"
