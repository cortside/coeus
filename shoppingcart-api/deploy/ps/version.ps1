#Docker variables
#$version = "1.0-local"
#$dotnetsdk = "6.0.302"
#$dotnetframework = "6.0.7"

$sdkimage = "cortside/dotnet-sdk:6.0-alpine"
$runtimeimage = "cortside/dotnet-runtime:6.0-alpine"

#Build variables
$projectname = "Acme.ShoppingCart" # needed for sln files
#$image = $projectname.ToLower() # needs to be lowercase
$image = "cortside/shoppingcart-api"
$sqlserver = "2019-latest"
$acr = "cortside"
#$v = "${version}"

#SonarQube
#$sonarscannerversion = "5.5.3"
$sonarkey = "acme_shoppingcart-api"
$localsonartoken = "123"
$localsonarhost = "https://sonarcloud.io"
$RepositorySlug = "shoppingcart-api"

$organization = "cortside"
$publishableProject = "Acme.ShoppingCart.WebApi"

$sonartoken = "123"
$sonarhost = "https://sonarcloud.io"
