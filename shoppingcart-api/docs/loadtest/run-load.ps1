$scriptPath = "load.ps1"
$numberOfScripts = 5

for ($i = 1; $i -le $numberOfScripts; $i++) {
  Start-Process pwsh.exe -ArgumentList "-NoExit", "-File", "$scriptPath"
  Write-Host "Started process $($i)"
}