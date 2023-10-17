[cmdletBinding()]
Param()

Push-Location "$PSScriptRoot/src/Acme.IdentityServer.WebApi"

cmd /c start cmd /k "title IdentityServer & dotnet run"

Pop-Location
