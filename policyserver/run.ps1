[cmdletBinding()]
Param()

Push-Location "$PSScriptRoot/src"

cmd /c start cmd /k "title PolicyServer & dotnet run"

Pop-Location
