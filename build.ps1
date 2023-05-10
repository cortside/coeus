./clean.ps1
docker compose down

$buildNumber = "10"
$branch = "develop"

docker images --format='{{json .}}'|Select-String -Pattern "cortside/shoppingcart-api" |   ConvertFrom-Json |  ForEach-Object -process { docker rmi -f $_.ID}
docker images --format='{{json .}}'|Select-String -Pattern "cortside/shoppingcart-web" |   ConvertFrom-Json |  ForEach-Object -process { docker rmi -f $_.ID}
docker images --format='{{json .}}'|Select-String -Pattern "cortside/identityserver6" |   ConvertFrom-Json |  ForEach-Object -process { docker rmi -f $_.ID}
docker images --format='{{json .}}'|Select-String -Pattern "cortside/policyserver" |   ConvertFrom-Json |  ForEach-Object -process { docker rmi -f $_.ID}

docker system prune --force 

docker pull cortside/dotnet-sdk:6.0-alpine
docker pull cortside/dotnet-runtime:6.0-alpine
docker pull cortside/ng-cli:14-alpine
docker pull cortside/ng-nginx:1.15-alpine

docker images | grep -i cortside

cd shoppingcart-api;
.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber;
cd ..;
cd shoppingcart-web;
.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber;
cd ..;
cd identityServer6;
.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber;
cd ..;
cd policyserver;
.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber;
cd ..;
