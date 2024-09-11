[cmdletBinding()]
param()



Try {
    Write-Host "Starting RabbitMQ"
    $rabbitMqDirVol = (Get-ChildItem -recurse -filter "rabbitmq").fullname.replace("\", "/")	
    docker rm --force $(docker stop $(docker ps -a -q --filter ancestor=rabbitmq:management-alpine --format="{{.ID}}"))
    docker run -d --name rabbitmq -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=password -v $rabbitMqDirVol/:/etc/rabbitmq -p 5671:5671 -p 5672:5672 -p 15672:15672 rabbitmq:management-alpine
    if ($LastExitCode -ne 0) {
        throw 
    }
    
    & "$PSScriptRoot/Set-RabbitQueues.ps1"
}
Catch {
    Write-Host "There was a problem starting the rabbitmq docker container"
    Write-Host "Please make sure you have docker installed"
    Write-Host "Make sure docker is running and that it is set to run linux containers"
    Write-Host $_
}
