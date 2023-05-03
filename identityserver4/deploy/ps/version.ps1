#Docker variables
$version = "1.0-local"
$dotnetsdk = "3.1"
$dotnetframework = "3.1"

#Build variables
$projectname = "Acme.IdentityServer" # needed for sln files
$image = $projectname.ToLower()
$acr = "Acme.azurecr.io"
$v = "${version}"

# sonar
$sonarscannerversion = "5.0.4"
$sonarkey = "Acme_Acme.identityserver"
$localsonartoken = "27cd4302ec52739df3edb5ebaa620bd995476e7b"
$localsonarhost = "https://sonarcloud.io"
