[cmdletBinding()]
Param()

Push-Location "$PSScriptRoot/src/Acme.ShoppingCart.WebApi"

cmd /c start cmd /k "title Acme.ShoppingCart.WebApi & dotnet run"

Pop-Location
