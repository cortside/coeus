[cmdletBinding()]
Param()

Push-Location "$PSScriptRoot"

if(!(Test-Path ".\src\config.local.json")) {
    New-Item -path ".\src" -name "config.local.json" -type "file" -value "{}"
}

cmd /c start cmd /k "title ShoppingCart Web - Angular & npm ci & npm run start"

Pop-Location
