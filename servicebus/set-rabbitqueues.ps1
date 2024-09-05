[cmdletBinding()]
param()


do {
    write-host "Waiting for RabbitMQ web portal to start..."
    start-sleep -Seconds 5
    # match on the pattern in the logs to make sure rabbit is ready to receive configuration
    $rabbitmqlogoutput = docker logs $(docker ps -a -q --filter="ancestor=rabbitmq:management-alpine") | sls -pattern "completed with .* plugins"
    if ($LastExitCode -ne 0) {
        throw 
    }
    Write-Host $rabbitmqlogoutput
} while ($rabbitmqlogoutput -eq $null)

Write-Host "Sending rabbit configuration"
$jsonBody = Get-Content -Raw -Path (Join-Path (Get-ChildItem -recurse -filter "rabbitmq").fullname "rabbit_configuration.json")
Invoke-RestMethod -Method Post -Uri "http://localhost:15672/api/definitions" -Headers @{Authorization = "Basic YWRtaW46cGFzc3dvcmQ=" } -ContentType "application/json" -Body $jsonBody
