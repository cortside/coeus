./clean.ps1

$buildNumber = "10"
$branch = "develop"

docker pull cortside/dotnet-sdk:6.0-alpine
docker pull cortside/dotnet-runtime:6.0-alpine
docker pull cortside/ng-cli:14-alpine
docker pull cortside/ng-nginx:1.15-alpine

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
