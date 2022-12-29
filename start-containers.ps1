docker rm -f mssql
docker rm -f rabbitmq
docker rm -f seq
docker rm -f portainer
docker rm -f redis
docker rm -f redisinsight

$networkName = "redis-net"
if (docker network ls | select-string $networkName -Quiet )
{
    Write-Host "$networkName already created"
} else {
    docker network create $networkName
}

$volumeName = "portainer_data"
if (docker volume ls | select-string $volumeName -Quiet )
{
    Write-Host "$volumeName already created"
} else {
    docker volume create $volumeName
}

#cat ~/.ssh/id_rsa.pub | ssh $($env:USERNAME)@$($env:DOCKER_HOST) "mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys"
$dest = "$($env:USERNAME)@$($env:DOCKER_HOST)"
scp './rabbitmq/*' "$($dest)`:/srv/rabbitmq/"

echo "starting sql"
#docker run --name mssql -d --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=password1@" -p 1433:1433 --hostname localhost -v /srv/mssql/data:/var/opt/mssql/data -v /srv/mssql/log:/var/opt/mssql/log -v /srv/mssql/secrets:/var/opt/mssql/secrets mcr.microsoft.com/mssql/server:2022-latest
docker run --user root --name mssql -d --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=password1@" -p 1433:1433 --hostname localhost -v /srv/mssql:/var/opt/mssql --detach mcr.microsoft.com/mssql/server:2022-latest /opt/mssql/bin/sqlservr -T 1800
echo "starting rabbit"
docker run --name rabbitmq -d --restart unless-stopped -v /srv/rabbitmq:/etc/rabbitmq --hostname rabbitmq -p 15672:15672 -p 5672:5672 -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=password rabbitmq:3-management-alpine
echo "starting seq"
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
echo "starting portainer"
docker run -d -p 8000:8000 -p 9443:9443 --name portainer --restart=always -v /var/run/docker.sock:/var/run/docker.sock -v portainer_data:/data portainer/portainer-ce:latest
echo "starting redis"
#docker run --name redis -d --net redis-net -v /srv/redis/data:/data -p 6379:6379 redis:7-alpine redis-server --save 60 1 --loglevel warning
docker run --name redis -d --net redis-net -v /srv/redislabs/data:/data -p 6379:6379 redislabs/redismod
echo "starting redisinsight"
docker run -d --name redisinsight --net redis-net -v /srv/redisinsight/db:/db -p 8001:8001 redislabs/redisinsight:latest

do {
    write-host "Waiting for RabbitMQ web portal to start..."
    start-sleep -Seconds 5
    # match on the pattern in the logs to make sure rabbit is ready to receive configuration
    $rabbitmqlogoutput = docker logs $(docker ps -a -q --filter="ancestor=rabbitmq:3-management-alpine") | sls -pattern "completed with .* plugins"
    if ($LastExitCode -ne 0) {
        throw 
    }
    Write-Host $rabbitmqlogoutput
} while ($rabbitmqlogoutput -eq $null)

Write-Host "Sending rabbit configuration"
$jsonBody = Get-Content -Raw -Path (Join-Path (Get-ChildItem -filter "rabbitmq").fullname "rabbit_configuration.json")
Invoke-RestMethod -Method Post -Uri "http://$($env:DOCKER_HOST)`:15672/api/definitions" -Headers @{Authorization = "Basic YWRtaW46cGFzc3dvcmQ=" } -ContentType "application/json" -Body $jsonBody

echo "containers started"
