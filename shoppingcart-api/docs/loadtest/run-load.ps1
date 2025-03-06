$scriptPath = "load.ps1"
$numberOfScripts = 10

for ($i = 1; $i -le $numberOfScripts; $i++) {
  Start-Process pwsh.exe -ArgumentList "-NoExit", "-File", "$scriptPath"
  Write-Host "Started process $($i)"
}