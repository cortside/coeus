[CmdletBinding()]
Param 
(
	[Parameter(Mandatory = $false)][switch]$tearDown
)

function envsubst {
  param([Parameter(ValueFromPipeline)][string]$InputObject)

  Get-ChildItem Env: | Set-Variable
  $ExecutionContext.InvokeCommand.ExpandString($InputObject)
}

if ($tearDown.IsPresent) {
	docker compose down

	docker volume rm coeus-data
	docker volume create coeus-data
	docker create -v coeus-data:/settings --name helper busybox true

	docker cp ./settings helper:/
	docker rm helper

	docker system prune --force
} else {
	docker create -v coeus-data:/settings --name helper busybox true
	docker cp ./settings helper:/
	docker rm helper
	docker compose restart
}

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
