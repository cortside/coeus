[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][string]$environment = "",
	[Parameter(Mandatory = $false)][switch]$tearDown
)

if ($environment -eq "") {
	$environment=$env:DOCKER_HOST
}

echo "*************"
echo "docker host: $($env:DOCKER_HOST)"
echo "environment: $environment"
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

	docker volume create coeus-data
	docker create -v coeus-data:/settings --name helper busybox true

	# todo: use for loop over directories
	docker cp "./settings/$environment/dashboard-web" helper:/settings
	docker cp "./settings/$environment/healthmonitor-api" helper:/settings
	docker cp "./settings/$environment/identityserver" helper:/settings
	docker cp "./settings/$environment/shoppingcart-api" helper:/settings
	docker cp "./settings/$environment/shoppingcart-web" helper:/settings

	docker rm helper

	docker system prune --force
} else {
	# todo: use for loop over directories
	docker compose cp "./settings/$environment/dashboard-web" identityserver:/settings
	docker compose cp "./settings/$environment/healthmonitor-api" identityserver:/settings
	docker compose cp "./settings/$environment/identityserver" identityserver:/settings
	docker compose cp "./settings/$environment/shoppingcart-api" identityserver:/settings
	docker compose cp "./settings/$environment/shoppingcart-web" identityserver:/settings

	docker compose restart
}

docker run --rm -i -v=coeus-data:/settings busybox ls -Al /settings

#docker compose pull
docker compose up -d

#docker exec -it coeus-healthmonitor-api-1 ls -R /settings
#docker exec -it coeus-healthmonitor-api-1 ls appsettings*

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
echo "done"
echo "*************"