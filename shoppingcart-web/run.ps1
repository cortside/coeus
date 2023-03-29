[cmdletBinding()]
Param()

Push-Location "$PSScriptRoot"

cmd /c start cmd /k "title ShoppingCart Web - Angular & npm ci & npm run build & npm run start"

Pop-Location
