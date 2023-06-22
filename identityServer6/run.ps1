[cmdletBinding()]
Param()

Push-Location "$PSScriptRoot/src"

cmd /c start cmd /k "title IdentityServer & dotnet run"

Pop-Location
