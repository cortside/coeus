./clean.ps1
docker compose down

$buildNumber = "10"
$branch = "develop"

docker images --format='{{json .}}'|Select-String -Pattern "cortside/shoppingcart-api" |   ConvertFrom-Json |  ForEach-Object -process { docker rmi -f $_.ID}
docker images --format='{{json .}}'|Select-String -Pattern "cortside/shoppingcart-web" |   ConvertFrom-Json |  ForEach-Object -process { docker rmi -f $_.ID}
docker images --format='{{json .}}'|Select-String -Pattern "cortside/identityserver4" |   ConvertFrom-Json |  ForEach-Object -process { docker rmi -f $_.ID}
docker images --format='{{json .}}'|Select-String -Pattern "cortside/policyserver" |   ConvertFrom-Json |  ForEach-Object -process { docker rmi -f $_.ID}

docker system prune --force 
docker image prune --all --force

docker pull cortside/dotnet-sdk:8.0-alpine
docker pull cortside/dotnet-runtime:8.0-alpine
docker pull cortside/dotnet-sdk:6.0-alpine
docker pull cortside/dotnet-runtime:6.0-alpine
docker pull cortside/ng-cli:16-alpine
docker pull cortside/ng-nginx:1.24-alpine

docker images | grep -i cortside

cd shoppingcart-api;
.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber;
cd ..;
cd shoppingcart-web;
.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber;
cd ..;
cd identityServer4;
.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber;
cd ..;
cd policyserver;
.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber;
cd ..;
