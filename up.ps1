function envsubst {
  param([Parameter(ValueFromPipeline)][string]$InputObject)

  Get-ChildItem Env: | Set-Variable
  $ExecutionContext.InvokeCommand.ExpandString($InputObject)
}

docker compose down

docker volume rm coeus-data
docker volume create coeus-data
docker create -v coeus-data:/settings --name helper busybox true

docker cp ./settings helper:/
docker rm helper

docker system prune --force

#docker compose pull
docker compose up -d

#docker exec -it coeus-healthmonitor-api-1 ls -R /settings
#docker exec -it coeus-healthmonitor-api-1 ls appsettings*