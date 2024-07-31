[cmdletBinding()]
Param()

$path = (gci -Recurse -Directory *.WebApi).Name
Push-Location "$PSScriptRoot/src/$path"

cmd /c start cmd /k "title $path & dotnet run"

Pop-Location
