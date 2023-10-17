param(
[Parameter(Mandatory=$false)]
[string]$extra_host,

[Parameter(Mandatory=$false)]
[string]$sql_server,

[Parameter(Mandatory=$false)]
[string]$sql_database,

[Parameter(Mandatory=$false)]
[string]$sql_password,

[Parameter(Mandatory=$false)]
[string]$dllname,

[Parameter(Mandatory=$false)]
[string]$sql_user,

[Parameter(Mandatory=$false)]
[string]$auth_authority
)


#show what network config looks like
ipconfig /all

#if(configdir) {
#	write-host "Loading local configs from: configdir"
#	$from = configdir.replace("/", "\")
#	if($from.endswith("\") -ne $true) { $from += "\" }
#	$from += "*"
#	cp $from c:\ -recurse -force
#} else {
#	write-host "Local configs not found, loading configs from consul (${env:consul}) with key 'config/files/${env:service}/${env:envtype}/'"
#	& c:/load-config.ps1
#
#	$consul = "c:/consul.exe"
#	$args = "watch -http-addr=${env:consul}:8500 -type=keyprefix -prefix=config/files/${env:service}/${env:envtype}/ powershell -File c:\load-config.ps1"
#	start-process $consul $args
#}

# append extra hosts to hosts files (since this does not work with extra_hosts in compose)
if ($EXTRA_HOST) { "$(EXTRA_HOST)" > c:\windows\system32\drivers\etc\hosts }

## update config from env vars
$config = gc c:\www\config.json -raw | ConvertFrom-json

$config.Database.ConnectionString = "Server=tcp:$($SQL_SERVER);Database=$($SQL_DATABASE);User Id=$($SQL_USER);Password=$($SQL_PASSWORD);Trusted_Connection=False;MultipleActiveResultSets=true"
$config.authentication.authority = "$($auth_authority)"
$config | convertto-json -depth 5 | out-file c:\www\config.json -force 

# start the service
cd c:\www
dotnet $dllname
