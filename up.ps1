[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][string]$environment = "",
	[Parameter(Mandatory = $false)][switch]$tearDown
)

$DOCKER_HOST = $env:DOCKER_HOST

if ($DOCKER_HOST -eq $null) { 
	$DOCKER_HOST="localhost"
	$environment=$DOCKER_HOST
}

if ($environment -eq "") {
	$environment=$DOCKER_HOST
}

echo "*************"
echo (Get-Date -Format "dddd MM/dd/yyyy HH:mm K")
echo "docker host: $($DOCKER_HOST)"
echo "environment: $environment"
echo "*************"


if ($DOCKER_HOST -eq $environment) {
	$environment = "DOCKER_HOST"
	echo "Using DOCKER_HOST files"
	echo "*************"
}

echo "DOCKER_HOST=$DOCKER_HOST" > .env
echo "DOCKER_HOST_IP=$((Resolve-DNSName -Type A $DOCKER_HOST).IPAddress)" >> .env
cat .env
echo "*************"

function envsubst {
  param([Parameter(ValueFromPipeline)][string]$InputObject)

  Get-ChildItem Env: | Set-Variable
  $ExecutionContext.InvokeCommand.ExpandString($InputObject)
}

if ($tearDown.IsPresent) {
	docker compose down --remove-orphans
	docker volume rm coeus-data
	docker compose down --remove-orphans
	docker volume prune --force
	docker volume list
	
	docker volume create coeus-data
	docker create -v coeus-data:/settings --name helper busybox:musl true

	# todo: use for loop over directories
	docker cp "./settings/$environment/dashboard-web" helper:/settings
	docker cp "./settings/$environment/healthmonitor-api" helper:/settings
	docker cp "./settings/$environment/identityserver" helper:/settings
	docker cp "./settings/$environment/policyserver" helper:/settings
	docker cp "./settings/$environment/mockserver" helper:/settings
	docker cp "./settings/$environment/shoppingcart-api" helper:/settings
	docker cp "./settings/$environment/shoppingcart-web" helper:/settings
	docker cp "./settings/$environment/sqlreport-api" helper:/settings

	docker rm helper
	docker system prune --force
} else {
	# todo: use for loop over directories
	docker compose cp "./settings/$environment/dashboard-web" identityserver:/settings
	docker compose cp "./settings/$environment/healthmonitor-api" identityserver:/settings
	docker compose cp "./settings/$environment/identityserver" identityserver:/settings
	docker compose cp "./settings/$environment/policyserver" identityserver:/settings
	docker compose cp "./settings/$environment/mockserver" identityserver:/settings
	docker compose cp "./settings/$environment/shoppingcart-api" identityserver:/settings
	docker compose cp "./settings/$environment/shoppingcart-web" identityserver:/settings
	docker compose cp "./settings/$environment/sqlreport-api" identityserver:/settings

	docker compose restart
}

docker run --rm -i -v=coeus-data:/settings busybox:musl ls -Al /settings
# replace DOCKER_HOST with env var value
$cmd = 'cd /settings; for i in $(grep -r -l ''DOCKER_HOST'' *); do sed -i ''s/DOCKER_HOST/env:DOCKER_HOST/g'' $i; echo $i; done'
$cmd = $cmd -replace 'env:DOCKER_HOST', $($DOCKER_HOST)
$cmd
docker run --rm -i -v=coeus-data:/settings busybox:musl sh -c $cmd
docker run --rm -i -v=coeus-data:/settings busybox:musl sh -c 'cd /settings; for i in $(find . -name ''*''); do echo $i; dos2unix -b $i; done'
docker run --rm -i -v=coeus-data:/settings busybox:musl ls -Al /settings

#docker compose pull
docker compose up -d

do {
    write-host "Waiting for bootstrap containers to finish..."
    start-sleep -Seconds 5
    # match on the pattern in the logs to make sure rabbit is ready to receive configuration
    $output = docker ps --format '{{.Names}}' | grep bootstrap 
    Write-Host $output
} while ($output -ne $null)

do {
    write-host "Waiting for all services to start..."
    start-sleep -Seconds 5
    # match on the pattern in the logs to make sure rabbit is ready to receive configuration
    $output = docker compose ps --format 'table {{.Names}} | {{.Status}}' | grep starting
    Write-Host $output
} while ($output -ne $null)

docker compose ps | grep starting

echo "*************"
echo (Get-Date -Format "dddd MM/dd/yyyy HH:mm K")
echo "done"
echo "*************"