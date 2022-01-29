[cmdletBinding()]
Param(
[string[]]$filePaths
)

Write-Output "===== USAGE EXAMPLE ====="
Write-Output  'PS C:\work\Acme.ShoppingCart> .\convert-encoding.ps1 -filePaths ((gci *.trigger.sql -Recurse) | % { $_.FullName })'
Write-Output "========================="


foreach ($file in $filePaths) {
    Write-Output "Convert encoding to UTF-8 for $file"
    $MyFile = Get-Content $file
    [System.IO.File]::WriteAllLines($file, $MyFile)
}
